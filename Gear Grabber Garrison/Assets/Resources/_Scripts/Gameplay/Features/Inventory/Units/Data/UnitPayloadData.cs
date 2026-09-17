using System;
using UnityEngine;

[Serializable]
public class UnitPayloadData
{
    public Guid ID;
    public Sprite Ava;
    public string Name;
    public string Effects;
    public long Price;
    [SerializeField] private string _guidShowOnly;

    public UnitPayloadData Clone()
    {
        return new UnitPayloadData
        {
            ID = this.ID,
            Ava = this.Ava,
            Name = this.Name,
            Effects = this.Effects,
            Price = this.Price
        };
    }

    public void SetID(Guid id)
    {
        ID = id;
    }

    public void SetID()
    {
        if(Guid.TryParse(_guidShowOnly, out Guid result))
        {
            ID = result;
        }
    }

    public void CreateGUID()
    {
        ID = Guid.NewGuid();
        _guidShowOnly = ID.ToString();
    }
}