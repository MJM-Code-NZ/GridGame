using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MJM
{
    public class Interface : MonoBehaviour
    {
        public static event EventHandler<OnUpdateCharacterMovementStateArgs> OnUpdateMovementState;
            
        private TMP_Dropdown _rulesDropdown;

        private UnityAction<int> _ruleChangeAction;

        void Awake()
        {
            _rulesDropdown = GameObject.Find("MovementRules/Dropdown").GetComponent<TMP_Dropdown>();

            var rulesOptions = new List<string>
            {
                CharacterMovementStateType.Idle.ToString(),
                CharacterMovementStateType.Wander.ToString(),
                CharacterMovementStateType.Target.ToString(),
            };

            _rulesDropdown.AddOptions(rulesOptions);

            _ruleChangeAction += OnRuleChange;

            _rulesDropdown.onValueChanged.AddListener(_ruleChangeAction);
        }

        private void OnRuleChange(int value)
        {
            var eventArgs = new OnUpdateCharacterMovementStateArgs
            {
                CharacterMovementStateType = (CharacterMovementStateType)value
            };

            OnUpdateMovementState?.Invoke(this, eventArgs);
        }
    }
}
