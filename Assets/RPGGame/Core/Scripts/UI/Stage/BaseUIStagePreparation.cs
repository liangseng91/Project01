﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUIStagePreparation<TUI, TStage> : UIDataItem<TStage>
    where TUI : UIDataItem<TStage>
    where TStage : BaseStage
{
    public UIFormation uiCurrentFormation;
    public UIItem uiFormationSlotPrefab;
    public TUI uiStage;
    public override void Clear()
    {
        // Don't clear
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public override void UpdateData()
    {
        if (uiStage != null)
            uiStage.SetData(data);
    }

    public override void Show()
    {
        base.Show();
        if (uiCurrentFormation != null)
        {
            uiCurrentFormation.formationName = Player.CurrentPlayer.SelectedFormation;
            uiCurrentFormation.SetFormationData(uiFormationSlotPrefab);
        }
    }
}
