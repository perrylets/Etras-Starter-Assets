using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EtrasStarterAssets
{

    public class PickupAbility : MonoBehaviour, ISerializationCallbackReceiver
    {
        public static List<string> TMPList;
        [HideInInspector] public List<string> abilityShortenedNames;
        [ListToPopup(typeof(PickupAbility), "TMPList")]
        public string Ability_To_Activate;
        private List<Ability> generalAbilities;
        EtraAbilityBaseClass selectedAbility;
        private void Start()
        {
            GetAllAbilities();
            Type abilityType = generalAbilities.ElementAt(abilityShortenedNames.IndexOf(Ability_To_Activate)).type;
            if ((EtraAbilityBaseClass)EtraCharacterMainController.Instance.etraAbilityManager.GetComponent(abilityType) == null)
            {
                Debug.LogWarning("PickupAbility.cs cannot activate the " + Ability_To_Activate + " ability on your character because your character does not have the " +  Ability_To_Activate + " script attached to its ability manager.");
            }
            else
            {
                selectedAbility = (EtraAbilityBaseClass)EtraCharacterMainController.Instance.etraAbilityManager.GetComponent(abilityType);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                selectedAbility.abilityEnabled = true;
                Destroy(this.gameObject);
            }
            
        }


        #region AbilityListDisplay
        public List<String> GetAllAbilities()
        {
            generalAbilities = new List<Ability>();
            generalAbilities = FindAllTypes<EtraAbilityBaseClass>().Select(x => new Ability(x)).ToList();

            List<string> temp = new List<string>();
            foreach (Ability ability in generalAbilities)
            {
                temp.Add(ability.shortenedName.ToString());
            }

           // temp.Sort();
            //generalAbilities.Sort();
            return temp;
        }

        public void OnBeforeSerialize()
        {
            abilityShortenedNames = GetAllAbilities();
            TMPList = abilityShortenedNames;
        }

        public void OnAfterDeserialize()
        {

        }

        public static IEnumerable<Type> FindAllTypes<T>()
        {
            var type = typeof(T);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t => t != type && type.IsAssignableFrom(t));
        }


        class Ability
        {
            public Ability(Type type)
            {
                this.type = type;
                state = false;
                name = type.Name;
                GenerateName();
            }

            public Type type;
            public string name;
            public string shortenedName;
            public bool state;

            public void GenerateName()
            {
                shortenedName = "";
                
                string[] splits = type.Name.Split('_');

                if (splits.Length == 2)
                {
                    shortenedName = splits[1];
                }
                else
                {
                    for (int i = 1; i < splits.Length; i++)
                    {
                        shortenedName += splits[i];
                        if (i!= splits.Length-1)
                        {
                            shortenedName += " ";
                        }
                        
                    }
                }

                shortenedName = Regex.Replace(shortenedName, "([a-z])([A-Z])", "$1 $2");
            }
        }
        #endregion
    }
}