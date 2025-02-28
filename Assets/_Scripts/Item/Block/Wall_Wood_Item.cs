using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Wood_Item : Item , IBuild
{
    public Com_ItemData _itemData_;
    public override ItemData Item_Data 
    {
        get
        {
            return _itemData_;
        }
        set
        {
            _itemData_ = (Com_ItemData)value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Use()
    {
        Build(transform.position, TheWorld.Instance.MapBlockParent);
    }
    [Button]
    public void Build(Vector3 position,Transform parent = null)
    {
       position = new Vector3(position.x, position.y, 0);
        print(position);
        if (position.x < 0)
        {
            print("x is less than 0");
            position.x -= 0.5f;
        }
        else if (position.x>0)
        {
            print("x is 1");
            position.x += 0.5f;
        }
       

        if (position.y < 0)
        {
            print("y is less than 0");
            position.y -= 0.5f;
        }else if (position.y > 0)
        {
            print("y is 1");
            position.y += 0.5f;
        }
        
        //È¡Õûºó+0.5f
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
       if (position.x < 0)
        {
            print("x is less than 0");
            position.x += 0.5f;
        }
        else if (position.x > 0)
        {
            print("x is 1");
            position.x -= 0.5f;
        }
        if (position.y < 0)
        {
            print("y is less than 0");
            position.y += 0.5f;
        }
        else if (position.y > 0)
        {
            print("y is 1");
            position.y -= 0.5f;
        }


        print(position);
      GameObject block = Instantiate(GameResManager.Instance.AllPrefabs[_itemData_.code], position, Quaternion.identity);
        if (parent!= null)
        {
            block.transform.parent = parent;
        }
    }
}

public interface IBuild
{
    void Build(Vector3 position, Transform parent = null);
}