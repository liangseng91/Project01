﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mono.Data.Sqlite;

public partial class SQLiteGameService
{
    protected override void DoLevelUpItem(string playerId, string loginToken, string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var foundPlayer = GetPlayerByLoginToken(playerId, loginToken);
        var foundItem = GetPlayerItemById(itemId);
        if (foundPlayer == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (foundItem == null || foundItem.PlayerId != playerId)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else
        {
            var softCurrency = GetCurrency(playerId, GameInstance.GameDatabase.softCurrency.id);
            var levelUpPrice = foundItem.LevelUpPrice;
            var requireCurrency = 0;
            var increasingExp = 0;
            var updateItems = new List<PlayerItem>();
            var deleteItemIds = new List<string>();
            var materialItemIds = materials.Keys;
            var materialItems = new List<PlayerItem>();
            foreach (var materialItemId in materialItemIds)
            {
                var foundMaterial = GetPlayerItemById(materialItemId);
                if (foundMaterial == null || foundMaterial.PlayerId != playerId)
                    continue;
                
                if (foundMaterial.CanBeMaterial)
                    materialItems.Add(foundMaterial);
            }
            foreach (var materialItem in materialItems)
            {
                var usingAmount = materials[materialItem.Id];
                if (usingAmount > materialItem.Amount)
                    usingAmount = materialItem.Amount;
                requireCurrency += levelUpPrice * usingAmount;
                increasingExp += materialItem.RewardExp * usingAmount;
                materialItem.Amount -= usingAmount;
                if (materialItem.Amount > 0)
                    updateItems.Add(materialItem);
                else
                    deleteItemIds.Add(materialItem.Id);
            }
            if (requireCurrency > softCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_SOFT_CURRENCY;
            else
            {
                softCurrency.Amount -= requireCurrency;
                ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                    new SqliteParameter("@amount", softCurrency.Amount),
                    new SqliteParameter("@id", softCurrency.Id));
                foundItem = foundItem.CreateLevelUpItem(increasingExp);
                updateItems.Add(foundItem);
                foreach (var updateItem in updateItems)
                {
                    ExecuteNonQuery(@"UPDATE playerItem SET playerId=@playerId, dataId=@dataId, amount=@amount, exp=@exp, equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                        new SqliteParameter("@playerId", updateItem.PlayerId),
                        new SqliteParameter("@dataId", updateItem.DataId),
                        new SqliteParameter("@amount", updateItem.Amount),
                        new SqliteParameter("@exp", updateItem.Exp),
                        new SqliteParameter("@equipItemId", updateItem.EquipItemId),
                        new SqliteParameter("@equipPosition", updateItem.EquipPosition),
                        new SqliteParameter("@id", updateItem.Id));
                }
                foreach (var deleteItemId in deleteItemIds)
                {
                    ExecuteNonQuery(@"DELETE FROM playerItem WHERE id=@id", new SqliteParameter("@id", deleteItemId));
                }
                result.updateCurrencies.Add(softCurrency);
                result.updateItems = updateItems;
                result.deleteItemIds = deleteItemIds;
            }
        }
        onFinish(result);
    }

    protected override void DoEvolveItem(string playerId, string loginToken, string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var foundPlayer = GetPlayerByLoginToken(playerId, loginToken);
        var foundItem = GetPlayerItemById(itemId);
        if (foundPlayer == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (foundItem == null || foundItem.PlayerId != playerId)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else
        {
            if (!foundItem.CanEvolve)
                result.error = GameServiceErrorCode.CANNOT_EVOLVE;
            else
            {
                var softCurrency = GetCurrency(playerId, GameInstance.GameDatabase.softCurrency.id);
                var requireCurrency = 0;
                var itemData = foundItem.ItemData;
                requireCurrency = foundItem.EvolvePrice;
                var enoughMaterials = true;
                var updateItems = new List<PlayerItem>();
                var deleteItemIds = new List<string>();
                var requiredMaterials = foundItem.EvolveMaterials;   // This is Key-Value Pair for `playerItem.DataId`, `Required Amount`
                var materialItemIds = materials.Keys;
                var materialItems = new List<PlayerItem>();
                foreach (var materialItemId in materialItemIds)
                {
                    var foundMaterial = GetPlayerItemById(materialItemId);
                    if (foundMaterial == null || foundMaterial.PlayerId != playerId)
                        continue;
                    
                    if (foundMaterial.CanBeMaterial)
                        materialItems.Add(foundMaterial);
                }
                foreach (var requiredMaterial in requiredMaterials)
                {
                    var dataId = requiredMaterial.Key;
                    var amount = requiredMaterial.Value;
                    foreach (var materialItem in materialItems)
                    {
                        if (materialItem.DataId != dataId)
                            continue;
                        var usingAmount = materials[materialItem.Id];
                        if (usingAmount > materialItem.Amount)
                            usingAmount = materialItem.Amount;
                        if (usingAmount > amount)
                            usingAmount = amount;
                        materialItem.Amount -= usingAmount;
                        amount -= usingAmount;
                        if (materialItem.Amount > 0)
                            updateItems.Add(materialItem);
                        else
                            deleteItemIds.Add(materialItem.Id);
                        if (amount == 0)
                            break;
                    }
                    if (amount > 0)
                    {
                        enoughMaterials = false;
                        break;
                    }
                }
                if (requireCurrency > softCurrency.Amount)
                    result.error = GameServiceErrorCode.NOT_ENOUGH_SOFT_CURRENCY;
                else if (!enoughMaterials)
                    result.error = GameServiceErrorCode.NOT_ENOUGH_ITEMS;
                else
                {
                    softCurrency.Amount -= requireCurrency;
                    ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                        new SqliteParameter("@amount", softCurrency.Amount),
                        new SqliteParameter("@id", softCurrency.Id));
                    foundItem = foundItem.CreateEvolveItem();
                    updateItems.Add(foundItem);
                    foreach (var updateItem in updateItems)
                    {
                        ExecuteNonQuery(@"UPDATE playerItem SET playerId=@playerId, dataId=@dataId, amount=@amount, exp=@exp, equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                            new SqliteParameter("@playerId", updateItem.PlayerId),
                            new SqliteParameter("@dataId", updateItem.DataId),
                            new SqliteParameter("@amount", updateItem.Amount),
                            new SqliteParameter("@exp", updateItem.Exp),
                            new SqliteParameter("@equipItemId", updateItem.EquipItemId),
                            new SqliteParameter("@equipPosition", updateItem.EquipPosition),
                            new SqliteParameter("@id", updateItem.Id));
                    }
                    foreach (var deleteItemId in deleteItemIds)
                    {
                        ExecuteNonQuery(@"DELETE FROM playerItem WHERE id=@id", new SqliteParameter("@id", deleteItemId));
                    }
                    result.updateCurrencies.Add(softCurrency);
                    result.updateItems = updateItems;
                    result.deleteItemIds = deleteItemIds;
                }
            }
        }
        onFinish(result);
    }

    protected override void DoSellItems(string playerId, string loginToken, Dictionary<string, int> items, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var player = GetPlayerByLoginToken(playerId, loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
        {
            var softCurrency = GetCurrency(playerId, GameInstance.GameDatabase.softCurrency.id);
            var returnCurrency = 0;
            var updateItems = new List<PlayerItem>();
            var deleteItemIds = new List<string>();
            var sellingItemIds = items.Keys;
            var sellingItems = new List<PlayerItem>();
            foreach (var sellingItemId in sellingItemIds)
            {
                var foundItem = GetPlayerItemById(sellingItemId);
                if (foundItem == null || foundItem.PlayerId != playerId)
                    continue;
                
                if (foundItem.CanSell)
                    sellingItems.Add(foundItem);
            }
            foreach (var sellingItem in sellingItems)
            {
                var usingAmount = items[sellingItem.Id];
                if (usingAmount > sellingItem.Amount)
                    usingAmount = sellingItem.Amount;
                returnCurrency += sellingItem.SellPrice * usingAmount;
                sellingItem.Amount -= usingAmount;
                if (sellingItem.Amount > 0)
                    updateItems.Add(sellingItem);
                else
                    deleteItemIds.Add(sellingItem.Id);
            }
            softCurrency.Amount += returnCurrency;
            ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                new SqliteParameter("@amount", softCurrency.Amount),
                new SqliteParameter("@id", softCurrency.Id));
            foreach (var updateItem in updateItems)
            {
                ExecuteNonQuery(@"UPDATE playerItem SET playerId=@playerId, dataId=@dataId, amount=@amount, exp=@exp, equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                    new SqliteParameter("@playerId", updateItem.PlayerId),
                    new SqliteParameter("@dataId", updateItem.DataId),
                    new SqliteParameter("@amount", updateItem.Amount),
                    new SqliteParameter("@exp", updateItem.Exp),
                    new SqliteParameter("@equipItemId", updateItem.EquipItemId),
                    new SqliteParameter("@equipPosition", updateItem.EquipPosition),
                    new SqliteParameter("@id", updateItem.Id));
            }
            foreach (var deleteItemId in deleteItemIds)
            {
                ExecuteNonQuery(@"DELETE FROM playerItem WHERE id=@id", new SqliteParameter("@id", deleteItemId));
            }
            result.updateCurrencies.Add(softCurrency);
            result.updateItems = updateItems;
            result.deleteItemIds = deleteItemIds;
        }
        onFinish(result);
    }

    protected override void DoEquipItem(string playerId, string loginToken, string characterId, string equipmentId, string equipPosition, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var foundPlayer = GetPlayerByLoginToken(playerId, loginToken);
        var foundCharacter = GetPlayerItemById(characterId);
        var foundEquipment = GetPlayerItemById(equipmentId);
        CharacterItem characterData = null;
        EquipmentItem equipmentData = null;
        if (foundCharacter != null)
            characterData = foundCharacter.CharacterData;
        if (foundEquipment != null)
            equipmentData = foundEquipment.EquipmentData;
        if (foundPlayer == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (foundCharacter == null || foundCharacter.PlayerId != playerId || foundEquipment == null || foundEquipment.PlayerId != playerId)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else if (characterData == null || equipmentData == null)
            result.error = GameServiceErrorCode.INVALID_ITEM_DATA;
        else if (!equipmentData.equippablePositions.Contains(equipPosition))
            result.error = GameServiceErrorCode.INVALID_EQUIP_POSITION;
        else
        {
            result.updateItems = new List<PlayerItem>();
            var unEquipItem = GetPlayerItemByEquipper(playerId, characterId, equipPosition);
            if (unEquipItem != null)
            {
                unEquipItem.EquipItemId = "";
                unEquipItem.EquipPosition = "";
                ExecuteNonQuery(@"UPDATE playerItem SET equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                    new SqliteParameter("@equipItemId", unEquipItem.EquipItemId),
                    new SqliteParameter("@equipPosition", unEquipItem.EquipPosition),
                    new SqliteParameter("@id", unEquipItem.Id));
                result.updateItems.Add(unEquipItem);
            }
            foundEquipment.EquipItemId = characterId;
            foundEquipment.EquipPosition = equipPosition;
            ExecuteNonQuery(@"UPDATE playerItem SET equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                new SqliteParameter("@equipItemId", foundEquipment.EquipItemId),
                new SqliteParameter("@equipPosition", foundEquipment.EquipPosition),
                new SqliteParameter("@id", foundEquipment.Id));
            result.updateItems.Add(foundEquipment);
        }
        onFinish(result);
    }

    protected override void DoUnEquipItem(string playerId, string loginToken, string equipmentId, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var player = GetPlayerByLoginToken(playerId, loginToken);
        var unEquipItem = GetPlayerItemById(equipmentId);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (unEquipItem == null || unEquipItem.PlayerId != playerId)
            result.error = GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA;
        else
        {
            result.updateItems = new List<PlayerItem>();
            unEquipItem.EquipItemId = "";
            unEquipItem.EquipPosition = "";
            ExecuteNonQuery(@"UPDATE playerItem SET equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                new SqliteParameter("@equipItemId", unEquipItem.EquipItemId),
                new SqliteParameter("@equipPosition", unEquipItem.EquipPosition),
                new SqliteParameter("@id", unEquipItem.Id));
            result.updateItems.Add(unEquipItem);
        }
        onFinish(result);
    }

    protected override void DoGetAvailableLootBoxList(UnityAction<AvailableLootBoxListResult> onFinish)
    {
        var result = new AvailableLootBoxListResult();
        var gameDb = GameInstance.GameDatabase;
        result.list.AddRange(gameDb.LootBoxes.Keys);
        onFinish(result);
    }

    protected override void DoOpenLootBox(string playerId, string loginToken, string lootBoxDataId, int packIndex, UnityAction<ItemResult> onFinish)
    {
        var result = new ItemResult();
        var gameDb = GameInstance.GameDatabase;
        var player = GetPlayerByLoginToken(playerId, loginToken);
        LootBox lootBox;
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.LootBoxes.TryGetValue(lootBoxDataId, out lootBox))
            result.error = GameServiceErrorCode.INVALID_LOOT_BOX_DATA;
        else
        {
            var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
            var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
            var requirementType = lootBox.requirementType;
            if (packIndex > lootBox.lootboxPacks.Length - 1)
                packIndex = 0;
            var pack = lootBox.lootboxPacks[packIndex];
            var price = pack.price;
            var openAmount = pack.openAmount;
            if (requirementType == LootBoxRequirementType.RequireSoftCurrency && price > softCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_SOFT_CURRENCY;
            else if (requirementType == LootBoxRequirementType.RequireHardCurrency && price > hardCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_HARD_CURRENCY;
            else
            {
                switch (requirementType)
                {
                    case LootBoxRequirementType.RequireSoftCurrency:
                        softCurrency.Amount -= price;
                        ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                            new SqliteParameter("@amount", softCurrency.Amount),
                            new SqliteParameter("@id", softCurrency.Id));
                        result.updateCurrencies.Add(softCurrency);
                        break;
                    case LootBoxRequirementType.RequireHardCurrency:
                        hardCurrency.Amount -= price;
                        ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
                            new SqliteParameter("@amount", hardCurrency.Amount),
                            new SqliteParameter("@id", hardCurrency.Id));
                        result.updateCurrencies.Add(hardCurrency);
                        break;
                }

                for (var i = 0; i < openAmount; ++i)
                {
                    var rewardItem = lootBox.RandomReward().rewardItem;
                    var createItems = new List<PlayerItem>();
                    var updateItems = new List<PlayerItem>();
                    if (AddItems(playerId, rewardItem.Id, rewardItem.amount, out createItems, out updateItems))
                    {

                        foreach (var createEntry in createItems)
                        {
                            createEntry.Id = System.Guid.NewGuid().ToString();
                            ExecuteNonQuery(@"INSERT INTO playerItem (id, playerId, dataId, amount, exp, equipItemId, equipPosition) VALUES (@id, @playerId, @dataId, @amount, @exp, @equipItemId, @equipPosition)",
                                new SqliteParameter("@id", createEntry.Id),
                                new SqliteParameter("@playerId", createEntry.PlayerId),
                                new SqliteParameter("@dataId", createEntry.DataId),
                                new SqliteParameter("@amount", createEntry.Amount),
                                new SqliteParameter("@exp", createEntry.Exp),
                                new SqliteParameter("@equipItemId", createEntry.EquipItemId),
                                new SqliteParameter("@equipPosition", createEntry.EquipPosition));
                            result.createItems.Add(createEntry);
                            HelperUnlockItem(player.Id, rewardItem.Id);
                        }
                        foreach (var updateEntry in updateItems)
                        {
                            ExecuteNonQuery(@"UPDATE playerItem SET playerId=@playerId, dataId=@dataId, amount=@amount, exp=@exp, equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                                new SqliteParameter("@playerId", updateEntry.PlayerId),
                                new SqliteParameter("@dataId", updateEntry.DataId),
                                new SqliteParameter("@amount", updateEntry.Amount),
                                new SqliteParameter("@exp", updateEntry.Exp),
                                new SqliteParameter("@equipItemId", updateEntry.EquipItemId),
                                new SqliteParameter("@equipPosition", updateEntry.EquipPosition),
                                new SqliteParameter("@id", updateEntry.Id));
                            result.updateItems.Add(updateEntry);
                        }
                    }
                }
            }
        }
        onFinish(result);
    }
}
