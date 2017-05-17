﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class CombatSubPhase : GenericSubPhase
    {

        public override void StartSubPhase()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Name = "Combat SubPhase";

            RequiredPlayer = Game.PhaseManager.PlayerWithInitiative;
            RequiredPilotSkill = GetStartingPilotSkill();

            Game.UI.AddTestLogEntry(Name);

            NextSubPhase();

        }

        public override void NextSubPhase()
        {
            Game.Selection.DeselectAllShips();

            Dictionary<int, Player> pilots = Game.Roster.NextPilotSkillAndPlayerAfter(RequiredPilotSkill, RequiredPlayer, Sorting.Desc);
            foreach (var pilot in pilots)
            {
                RequiredPilotSkill = pilot.Key;
                RequiredPlayer = pilot.Value;
            }

            UpdateHelpInfo();

            if (RequiredPilotSkill == -1)
            {
                Game.PhaseManager.CurrentPhase.NextPhase();
            }
            else
            {
                Game.Roster.GetPlayer(RequiredPlayer).PerformAttack();
            }
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            if ((ship.PlayerNo == RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill))
            {
                result = true;
            }
            else
            {
                Game.UI.ShowError("Ship cannot be selected:\n Need " + RequiredPlayer + " and pilot skill " + RequiredPilotSkill);
            }
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip targetShip)
        {
            bool result = false;
            if (Game.Selection.ThisShip != null)
            {
                if (targetShip.PlayerNo != Game.PhaseManager.CurrentSubPhase.RequiredPlayer)
                {
                    result = true;
                }
                else
                {
                    Game.UI.ShowError("Ship cannot be selected as attack target: Friendly ship");
                }
            }
            else
            {
                Game.UI.ShowError("Ship cannot be selected as attack target:\nFirst select attacker");
            }
            return result;
        }

        public override int CountActiveButtons(Ship.GenericShip ship)
        {
            int result = 0;
            if (Game.Selection.ThisShip != null)
            {
                if (ship.PlayerNo != Game.PhaseManager.CurrentSubPhase.RequiredPlayer)
                {
                    if (Game.Selection.ThisShip.IsAttackPerformed != true)
                    {
                        Game.UI.panelContextMenu.transform.Find("FireButton").gameObject.SetActive(true);
                        result++;
                    }
                    else
                    {
                        Game.UI.ShowError("Your ship already has attacked");
                    }
                }
            }
            return result;
        }

        public override int GetStartingPilotSkill()
        {
            return PILOTSKILL_MAX + 1;
        }

    }

}
