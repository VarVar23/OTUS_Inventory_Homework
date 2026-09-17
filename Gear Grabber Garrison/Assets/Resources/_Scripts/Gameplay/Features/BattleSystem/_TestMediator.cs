using System;
using Gameplay.Itemization;
using UnityEngine;
using Zenject;

namespace Gameplay.Features.Battlesystem
{
    // public enum ModificationType
    // {
    //     Add,
    //     Multiply,
    //     Set
    // }
    public class _TestMediator : MonoBehaviour
    {
        [Inject] private IBattleFacade _battleFacade;
        // [Inject] private IBattleFacade _itemizator;
        private Unit unit;
        private Guid _unitId;
        

        private void Start()
        {
            _battleFacade.OnEnemyDeath += HandleEnemyDeath;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                _unitId = Guid.NewGuid(); //TEMP
                Debug.Log("..эмитация1!!! жмем создать юнита, Guid получаем от инвентаря, допусит тестово пришел №..."+_unitId);
                _battleFacade.CreatePlayerUnit(_unitId);
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                
                //  имитация, юзер экипирует юнита с Guid, добавляет item c Guid;
                //  Действия совершены в инвентаре, инвентарь отдает Айтемизатору юнит Guid = 1, и item Guid = 100;
                //
                Debug.Log("..эмитация2!!! В инвентаре экипируем юнита Guid №" + _unitId + " айтомизатор вызывает Upgrade...");
                
                Func<float,float> valueChangeAction = ValueChangeMapping(ModificationType.Multiply, 2);
                _battleFacade.UpgradePlayerArmy(_unitId, StatType.AttackMaxDamage, valueChangeAction);
                
                // _itemizator.GetItemEffect(Unit[1], Item[4].GetEffect())
                // UnitUpdateAtributes(guid);
            }
            _battleFacade.DoSystemTick(Time.deltaTime);
        }

        
        private void HandleEnemyDeath(Guid unitId)
        {
            Debug.Log("[_TestMediator] Enamy is Dead! Unit ID: "+unitId);
            //_itemizator.getDrop();
        }
        
        
        private Func<float,float> ValueChangeMapping(ModificationType modificationType, float value)
        {
            return modificationType switch
            {
                ModificationType.Add     => v => v + value,
                ModificationType.Multiply=> v => v * value,
                ModificationType.Set     => v => value,
                _                        => v => v // по умолчанию оставляем unchanged
            };
        }
    }
}