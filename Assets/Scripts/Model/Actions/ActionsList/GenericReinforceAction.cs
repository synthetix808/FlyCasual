﻿using System.Linq;
using UnityEngine;

namespace ActionsList
{

    public class GenericReinforceAction : GenericAction
    {
        public GenericReinforceAction()
        {
            Name = EffectName = "Reinforce (Generic)";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReinforceAction.png";
        }

        public override void ActionTake()
        {
            Selection.ThisShip.OnTryConfirmDiceResults += CheckMustUseReinforce;
        }

        private void CheckMustUseReinforce(ref bool result)
        {
            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (Combat.Defender.GetAvailableActionEffectsList().Any(n => n.GetType().BaseType == typeof(GenericReinforceAction)))
                {
                    Messages.ShowError("Cannot confirm results - must spend reinforce token!");
                    result = false;
                }
            }
        }

        public override void ActionEffect(System.Action callBack)
        {
            Selection.ThisShip.OnTryConfirmDiceResults -= CheckMustUseReinforce;
            Combat.CurrentDiceRoll.ApplyEvade();
            callBack();
        }

        public override bool IsActionAvailable()
        {
            bool result = true;
            if ((Host.IsAlreadyExecutedAction(typeof(ReinforceForeAction))) || (Host.IsAlreadyExecutedAction(typeof(ReinforceAftAction))))
            {
                result = false;
            };
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccesses = Combat.DiceRollAttack.Successes;
                int defenceSuccesses = Combat.DiceRollDefence.Successes;
                if (attackSuccesses > defenceSuccesses)
                {
                    result = 110;
                }
            }

            return result;
        }

    }

}
