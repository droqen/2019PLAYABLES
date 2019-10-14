namespace navdi3.xxi
{
    using UnityEngine;
    using UnityEngine.Tilemaps;
    using System.Collections;
    using System.Collections.Generic;

    [RequireComponent(typeof(BankLot))]
    [RequireComponent(typeof(SpriteLot))]

    abstract public class BaseSimpleXXI : MonoBehaviour
    {
        public BankLot banks { get { return GetComponent<BankLot>(); } }
        public SpriteLot sprites { get { return GetComponent<SpriteLot>(); } }

        protected Dictionary<string, EntityLot> entlots = new Dictionary<string, EntityLot>();
        public EntityLot GetEntLot(string entLotName)
        {
            if (!entlots.ContainsKey(entLotName)) entlots.Add(entLotName, EntityLot.NewEntLot(entLotName));
            return entlots[entLotName];
        }
    }
}