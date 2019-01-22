using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers
{

    /// This is a starter template for Save System savers. To use it,
    /// make a copy, rename it, and remove the line marked above.
    /// Then fill in your code where indicated below.

    //Saves a gameObject's reputation and other levelling values
    [AddComponentMenu("")]
    public class StatusSaver : Saver // Rename this class.
    {
        [Serializable]
        public class RepuationInfo
        {
            public float reputation;
            public float health;
        }

        private RepuationInfo m_data = new RepuationInfo();

        public override string RecordData()
        {
            /// This method should return a string that represents the data you want to save.
            /// You can use SaveSystem.Serialize() to serialize a serializable object to a 
            /// string. This will use the serializer component on the Save System GameObject,
            /// which defaults to JSON serialization.
            /// 
            m_data.reputation = GetComponent<Reputable>().Reputation;
            m_data.health = GetComponent<HealthManager>().Health;
            return SaveSystem.Serialize(m_data);
        }

        public override void ApplyData(string s)
        {
            /// This method should process the string representation of saved data and apply
            /// it to the current state of the game. You can use SaveSystem.Deserialize()
            /// to deserialize the string to an object that specifies the state to apply to
            /// the game.
            /// 
             if (!string.IsNullOrEmpty(s))
            {
                var data = SaveSystem.Deserialize<RepuationInfo>(s, m_data);
                if (data == null) return;
                m_data = data;
                GetComponent<Reputable>().Reputation = data.reputation;
                GetComponent<HealthManager>().SetHealthFromSave(data.health);
            }
        }

        //public override void OnBeforeSceneChange()
        //{
        //    // The Save System will call this method before scene changes. If your saver listens for 
        //    // OnDisable or OnDestroy messages (see DestructibleSaver for example), it can use this 
        //    // method to ignore the next OnDisable or OnDestroy message since they will be called
        //    // because the entire scene is being unloaded.
        //}

    }

}

/**/
