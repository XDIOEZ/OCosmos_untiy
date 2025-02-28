/*using NPOI.XSSF.UserModel; // ���ڲ���Excel�ļ���.xlsx��
using NPOI.SS.UserModel;   // �ṩExcel����ͨ�ýӿ�
using System.IO;           // �ļ�������ص������ռ�
using UnityEngine;         // Unity�ĺ��������ռ�
using UnityEditor;         // ���ڱ༭�����ܵ������ռ�
using UnityEditor.AddressableAssets.Settings; // ���ڲ���Addressable��Դ������
using UnityEditor.AddressableAssets;          // Addressable��Դ�������
using Sirenix.OdinInspector;                  // Odin Inspector�����������ǿ�༭������
using System.Collections.Generic;
using System;

/// <summary>
/// ͨ��NPOIʵ��Excel��Unity Prefab���ݵĶ�д�����Ĺ�������
/// ֧�ֶ�ȡExcel����Prefab�ͽ�Prefab����д��Excel��
/// </summary>
public class ExcelManager : MonoBehaviour
{
    /// <summary>
    /// Excel�ļ�·�������û�ͨ��Inspector���á�
    /// </summary>
    public string ExcelPath;
    public string NewPrefabPath = "NewPrefabPath";


    
    /// <summary>
    /// Addressable��Դ�����ã����ڲ������е�Prefab��
    /// </summary>
    public AddressableAssetGroup ADBGroup;

    /// <summary>
    /// ��Excel��ȡ���ݲ�����Prefab�е�������ݡ�
    /// ʹ��Odin Inspector��[Button]���ԣ������ڱ༭����ͨ����ť������
    /// </summary>
    /// 
    [Button]
    #region ��ȡExcel���ݲ�����Prefabs
    /// <summary>
    /// ��ȡָ����Excel������������Excel���ݸ���Prefab���ԣ�
    /// ���Prefab�����ڣ��򴴽��µ�Prefab�����ձ���Excel�ļ���
    /// </summary>
    /// <param name="OpenExcelSheetName">Ҫ�򿪵Ĺ��������ƣ�Ĭ��Ϊ "WeaponData"��</param>
    public void ReadExcel(string OpenExcelSheetName = "WeaponData")
    {
        // ��֤Excel·���Ƿ���Ч�������Ч���˳�
        if (!ValidateExcelPath()) return;

        // ����Excel������
        IWorkbook workbook = LoadWorkbook(ExcelPath);
        if (workbook == null) return;  // �������ʧ�ܣ��˳�

        // ����ָ�����ƵĹ�����
        ISheet sheet = LoadSheet(workbook, OpenExcelSheetName);
        if (sheet == null) return;  // ������������ʧ�ܣ��˳�

        // ����Prefab�ֵ䣬ʹ�� ID ��Ϊ��
        Dictionary<int, GameObject> prefabDictionary = BuildPrefabDictionary();

        // ����Excel���ݣ��������ݸ���Prefab�򴴽��µ�Prefab
        UpdateOrCreatePrefabs(sheet, prefabDictionary, OpenExcelSheetName);

        // �����޸ĺ��Excel������
        SaveWorkbook(workbook, ExcelPath);

        Debug.Log("Excel��ȡ��ɣ�");
    }
    #endregion


    #region ����Prefab����
    /// <summary>
    /// ����Excel���ݸ���Prefab�����ԡ�
    /// </summary>
    /// <param name="row">Excel�����ݡ�</param>
    /// <param name="prefab">��Ҫ���µ�Prefab��</param>
    /// <param name="sheetName">��ǰ���������ƣ����������������͡�</param>
    private void UpdatePrefabProperties(IRow row, GameObject prefab, string sheetName)
    {
        Item willChangeItem = prefab.GetComponent<Item>();
        if (willChangeItem != null && willChangeItem.GetData() != null)
        {
            //0.A
            int i = 0;
            //1.B
            willChangeItem.GetData().Name = row.GetCell(++i)?.ToString();
            //2.C
            willChangeItem.GetData().Description = row.GetCell(++i)?.ToString();
            //3.D
            willChangeItem.GetData().PrefabPath = row.GetCell(++i)?.ToString() ?? AssetDatabase.GetAssetPath(willChangeItem.gameObject);
               
            //4.E
            willChangeItem.GetData().Volume = float.Parse(row.GetCell(++i)?.ToString());
            //5.DropItem
            willChangeItem.GetData().Durability = float.Parse(row.GetCell(++i)?.ToString());
            //6.G
            willChangeItem.GetData().CanBePickedUp = bool.Parse(row.GetCell(++i)?.ToString());
            //7.H
            willChangeItem.GetData().ItemTag.Type_Base = (row.GetCell(++i)?.ToString());

            if (sheetName == "WeaponData")
            {
                Weapon willChangeItem_weapon = (Weapon)willChangeItem;
                
                willChangeItem_weapon._Data._damage = float.Parse(row.GetCell(++i)?.ToString());
                
                willChangeItem_weapon._Data.StaminaCost = float.Parse(row.GetCell(++i)?.ToString());
                
                willChangeItem_weapon._Data.MaxAttackDistance = float.Parse(row.GetCell(++i)?.ToString());
                
                willChangeItem_weapon._Data.AttackSpeed = float.Parse(row.GetCell(++i)?.ToString());
                
                willChangeItem_weapon._Data.ReturnSpeed = float.Parse(row.GetCell(++i)?.ToString());
            }

            EditorUtility.SetDirty(prefab);
            Debug.Log($"Prefab ID: {willChangeItem.GetData().ID} ���³ɹ���");
        }
    }
    #endregion

    #region ��֤Excel·��
    /// <summary>
    /// ��֤Excel·���Ƿ�����ȷ���á�
    /// </summary>
    /// <returns>·����Ч����true����Ч����false��</returns>
    private bool ValidateExcelPath()
    {
        if (string.IsNullOrEmpty(ExcelPath))
        {
            Debug.LogError("Excel·��δ���ã�");
            return false;
        }
        return true;
    }
    #endregion

    #region ���»򴴽�Prefabs
    /// <summary>
    /// ����Excel�е����ݣ�����ID���¶�Ӧ��Prefab���ԣ�
    /// ���ID�����ڣ��򴴽��µ�Prefab��
    /// </summary>
    /// <param name="sheet">Excel���������</param>
    /// <param name="prefabDictionary">����Prefab�ֵ䡣</param>
    /// <param name="sheetName">��ǰ���������ơ�</param>
    private void UpdateOrCreatePrefabs(ISheet sheet, Dictionary<int, GameObject> prefabDictionary, string sheetName)
    {
        for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
        {
            IRow row = sheet.GetRow(rowIndex);
            if (row == null) continue;

            // ���Ա�����
            if (rowIndex == 0) continue;

            // ��ȡPrefab��ID
            string prefabIdString = row.GetCell(0)?.ToString().Trim();
            if (int.TryParse(prefabIdString, out int prefabId))
            {
                if (prefabDictionary.TryGetValue(prefabId, out GameObject prefab))
                {
                    // ��������Prefab
                    UpdatePrefabProperties(row, prefab, sheetName);
                }
                else
                {
                    // ������Prefab
                    CreateNewPrefab(row, prefabId, sheetName);
                }
            }
            else
            {
                Debug.LogWarning($"�� {rowIndex} ��ID��ʽ�����������С�");
            }
        }
    }
    #endregion

    #region ����Excel������
    /// <summary>
    /// ���޸ĺ��Excel���������浽ָ��·����
    /// </summary>
    /// <param name="workbook">��Ҫ����Ĺ���������</param>
    /// <param name="path">����·����</param>
    private void SaveWorkbook(IWorkbook workbook, string path)
    {
        try
        {
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(file);
                Debug.Log("Excel�ļ��ѳɹ����棡");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"����Excel�ļ�ʧ�ܣ�{e.Message}");
        }
    }
    #endregion

    #region ������Prefab
    /// <summary>
    /// ����һ���µ�Prefab���������ʼ���ԡ�
    /// </summary>
    /// <param name="row">Excel�����ݡ�</param>
    /// <param name="prefabId">Prefab��ΨһID��</param>
    /// <param name="sheetName">��ǰ���������ơ�</param>
    private void CreateNewPrefab(IRow row, int prefabId, string sheetName)
    {
        GameObject newGameObject = new GameObject();
        Weapon weapon = newGameObject.AddComponent<Weapon>();

        // ��ʼ������
*//*        weapon._Data = new WeaponData
        {
            ID = prefabId,
            Name = row.GetCell(1)?.ToString(),
            Description = row.GetCell(2)?.ToString(),
            PrefabPath = row.GetCell(3)?.ToString(),
            Volume = float.Parse(row.GetCell(4)?.ToString()),
            Durability = float.Parse(row.GetCell(5)?.ToString())
        };*//*

        if (sheetName == "WeaponData")
        {
            weapon._Data._damage = float.Parse(row.GetCell(6)?.ToString());
            weapon._Data.StaminaCost = float.Parse(row.GetCell(7)?.ToString());
            weapon._Data.MaxAttackDistance = float.Parse(row.GetCell(8)?.ToString());
            weapon._Data.AttackSpeed = float.Parse(row.GetCell(9)?.ToString());
            weapon._Data.ReturnSpeed = float.Parse(row.GetCell(10)?.ToString());
        }

        // ����Prefab
        string path = AssetDatabase.GenerateUniqueAssetPath(NewPrefabPath + weapon._Data.Name + ".prefab");
        PrefabUtility.SaveAsPrefabAsset(newGameObject, path);

        Debug.Log($"�ɹ�������Prefab: {path}");
        DestroyImmediate(newGameObject);
    }
    #endregion

    #region ����Excel�ļ�
    /// <summary>
    /// ����Excel�ļ������ع���������
    /// </summary>
    /// <param name="path">Excel�ļ�·����</param>
    /// <returns>���سɹ����ع��������󣬷��򷵻�null��</returns>
    private IWorkbook LoadWorkbook(string path)
    {
        try
        {
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Debug.Log("Excel�ļ��ɹ����أ�");
                return new XSSFWorkbook(file); // ��.xlsx�ļ�
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"����Excel�ļ�ʧ�ܣ�{e.Message}");
            return null;
        }
    }
    #endregion

    #region ���ع�����
    /// <summary>
    /// �������Ƽ���ָ���Ĺ�����
    /// </summary>
    /// <param name="workbook">Excel����������</param>
    /// <param name="sheetName">���������ơ�</param>
    /// <returns>���سɹ����ع�������󣬷��򷵻�null��</returns>
    private ISheet LoadSheet(IWorkbook workbook, string sheetName)
    {
        ISheet sheet = sheetName != "null" ? workbook.GetSheet(sheetName) : workbook.GetSheetAt(0);
        if (sheet == null)
        {
            Debug.LogError($"δ�ҵ���Ϊ {sheetName} �Ĺ�����");
            return null;
        }
        Debug.Log($"�ɹ����ع�����: {sheetName}");
        return sheet;
    }
    #endregion

    #region ����Prefab�ֵ�
    /// <summary>
    /// ����Prefab�ֵ䣬ʹ��Prefab�� ID ��Ϊ����
    /// </summary>
    /// <returns>���ذ���Prefab���ֵ䡣</returns>
    private Dictionary<int, GameObject> BuildPrefabDictionary()
    {
        Dictionary<int, GameObject> prefabDictionary = new Dictionary<int, GameObject>();

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings δ�ҵ���");
            return prefabDictionary;
        }

        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group == null || group.entries.Count == 0) continue;

            foreach (var entry in group.entries)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.AssetPath);
                if (prefab == null) continue;

                Weapon weapon = prefab.GetComponent<Weapon>();
                if (weapon != null && weapon._Data != null)
                {
                    prefabDictionary[weapon._Data.ID] = prefab;
                    Debug.Log($"�ɹ���Prefab ID:{weapon._Data.ID} �����ֵ�");
                }
            }
        }

        return prefabDictionary;
    }
    #endregion




    [Button]
    public void ReadExcelByStringKey(string OpenExcelSheetName = "WeaponData")
    {
        // ����һ���ֵ����ڴ洢Prefab���� `willChangeItem.GetData().ID` ��Ϊ��
        Dictionary<int, GameObject> prefabDictionary = new Dictionary<int, GameObject>();
        List<string> ExcelFirstRow = new List<string>();//��˳�򱣴�Excel��һ������

        // ���Excel·���Ƿ���Ч
        if (string.IsNullOrEmpty(ExcelPath))
        {
            Debug.LogError("Excel·��δ���ã�");
            return;
        }

        IWorkbook workbook;

        // ��ȡExcel�ļ�����������
        using (FileStream file = new FileStream(ExcelPath, FileMode.Open, FileAccess.Read))
        {
            workbook = new XSSFWorkbook(file); // ��.xlsx�ļ�
            Debug.Log("Excel�ļ��ɹ����أ�");

            // ���ָ���˹��������ƣ����ȡ��Ӧ�Ĺ���������Ĭ�ϻ�ȡ��һ��������
            ISheet sheet = OpenExcelSheetName != "null" ? workbook.GetSheet(OpenExcelSheetName) : workbook.GetSheetAt(0);
            if (sheet == null)
            {
                Debug.LogError($"δ�ҵ���Ϊ {OpenExcelSheetName} �Ĺ�����");
                return;
            }

            Debug.Log($"�ɹ����ع�����: {OpenExcelSheetName}");

            // ��ȡAddressable��Դ����
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings δ�ҵ���");
                return;
            }

            // ������Դ�飬��Prefab�����ֵ�
            Debug.Log("��ʼ����Addressable��Դ��...");
            foreach (AddressableAssetGroup group in settings.groups)
            {
                if (group == null || group.entries.Count == 0) continue;

                foreach (var entry in group.entries)
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.AssetPath); // ����Prefab
                    if (prefab == null) continue;

                    Weapon weapon = prefab.GetComponent<Weapon>(); // ��ȡPrefab�ϵ�Weapon���
                    if (weapon != null)
                    {
                        // ��Prefab�� `ID` ��Ϊ�������ֵ�
                        if (weapon.GetData() != null)
                        {
                            prefabDictionary[weapon.GetData().ID] = prefab;
                            Debug.Log($"�ɹ���Prefab ID:{weapon.GetData().ID} �����ֵ�");
                        }
                    }
                }
            }

            // ����Excel�е����ݲ����¶�Ӧ��Prefab
            Debug.Log("��ʼ��ȡExcel���ݲ�����Prefab...");
            for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row == null) continue;


                // TODO ��� ExcelFirstRow
                // ���Ա�����
                if (rowIndex == 0)
                {
                    Debug.Log("����������...");
                    continue;
                }

                // ��ȡPrefab�� `ID`��������Excel�ĵ�һ�У�
                string prefabIdString = row.GetCell(0)?.ToString().Trim(); // ȥ������Ŀո�
                if (int.TryParse(prefabIdString, out int prefabId))
                {
                    Debug.Log($"����Prefab ID: {prefabId}");

                    // ����ֵ��д��ڶ�Ӧ��Prefab�������������
                    if (prefabDictionary.TryGetValue(prefabId, out GameObject prefab))
                    {
                        Weapon weapon = prefab.GetComponent<Weapon>();
                        if (weapon != null && weapon.GetData() != null)
                        {

                            //����
                            weapon.GetData().Name = row.GetCell(1)?.ToString();

                            //����
                            weapon.GetData().Description = row.GetCell(2)?.ToString();

                            //Ԥ�Ƽ�·��
                            if (row.GetCell(3)?.ToString() == null)
                            {
                                //��ȡ willChangeItem.gameObjectԤ�Ƽ�·��
                                weapon.GetData().PrefabPath = AssetDatabase.GetAssetPath(weapon.gameObject);
                                Debug.Log("Prefab·��Ϊ�գ����Զ���ȡԤ�Ƽ�·����" + weapon.GetData().PrefabPath);
                                row.CreateCell(3).SetCellValue(weapon._Data.PrefabPath);
                                //����excel

                            }
                            else
                            {
                                weapon._Data.PrefabPath = row.GetCell(3)?.ToString();
                                Debug.Log("Prefab·���Ѹ��£�" + weapon._Data.PrefabPath);
                            }

                            //�������
                            weapon._Data.Volume = float.Parse(row.GetCell(4)?.ToString());

                            //�;ö�
                            weapon._Data.Durability = float.Parse(row.GetCell(5)?.ToString());


                            if (OpenExcelSheetName == "WeaponData")
                            {
                                //�˺�
                                weapon._Data._damage = float.Parse(row.GetCell(6)?.ToString());
                                //���������ٶ�
                                weapon._Data.StaminaCost = float.Parse(row.GetCell(7)?.ToString());
                                //��󹥻�����
                                weapon._Data.MaxAttackDistance = float.Parse(row.GetCell(8)?.ToString());
                                //���幥���ٶ�
                                weapon._Data.AttackSpeed = float.Parse(row.GetCell(9)?.ToString());
                                //���巵���ٶ�
                                weapon._Data.ReturnSpeed = float.Parse(row.GetCell(10)?.ToString());
                            }





                            // ���PrefabΪ���޸�
                            EditorUtility.SetDirty(prefab);
                            Debug.Log($"Prefab ID: {prefabId} ���³ɹ���");


                        }
                    }
                    else
                    {
                        Debug.LogWarning($"δ�ҵ�Prefab IDΪ {prefabId} ��Prefab��");
                        #region ������Prefab

                        // ����һ���µ� GameObject ��Ϊ����� Weapon ���
                        GameObject newGameObject = new GameObject();  // �����µ� GameObject
                        Weapon weapon = newGameObject.AddComponent<Weapon>();  // Ϊ GameObject ��� Weapon ���
                        GameObject newGameObject_Sprite = new GameObject();  // �����µ� GameObject
                        newGameObject_Sprite.AddComponent<SpriteRenderer>();  // Ϊ GameObject ��� SpriteRenderer ���
                        newGameObject_Sprite.transform.SetParent(newGameObject.transform);  // ���ø�����
                        newGameObject_Sprite.transform.localPosition = Vector3.zero;  // ����λ��
                        newGameObject_Sprite.transform.localScale = Vector3.one;  // ��������
                        #endregion

                        #region ���� Data ���

                        // ��ʼ�� WeaponData ���
                     *//*   weapon._Data = new WeaponData();  // ��ʼ������*//*
                        weapon._Data.ID = prefabId;
                        weapon._Data.Name = row.GetCell(1)?.ToString();
                        weapon.gameObject.name = weapon._Data.Name;
                        weapon._Data.Description = row.GetCell(2)?.ToString();
                        weapon._Data.PrefabPath = row.GetCell(3)?.ToString();
                        weapon._Data.Volume = float.Parse(row.GetCell(4)?.ToString());
                        weapon._Data.Durability = float.Parse(row.GetCell(5)?.ToString());
                        if (OpenExcelSheetName == "WeaponData")
                        {
                            weapon._Data._damage = float.Parse(row.GetCell(6)?.ToString());
                            weapon._Data.StaminaCost = float.Parse(row.GetCell(7)?.ToString());
                            weapon._Data.MaxAttackDistance = float.Parse(row.GetCell(8)?.ToString());
                            weapon._Data.AttackSpeed = float.Parse(row.GetCell(9)?.ToString());
                            weapon._Data.ReturnSpeed = float.Parse(row.GetCell(10)?.ToString());
                        }
                        #endregion

                        #region ����Prefab
                        // ���� Prefab �����浽ָ��·��
                        string path = AssetDatabase.GenerateUniqueAssetPath(NewPrefabPath + weapon.gameObject.name + ".prefab");
                        PrefabUtility.SaveAsPrefabAsset(newGameObject, path);  // �� GameObject ����Ϊ Prefab

                        // TODO �� Prefab ���� Addressable ��Դ��
                        AddressableAssetSettings ADBSettings = AddressableAssetSettingsDefaultObject.Settings;
                        AddressableAssetGroup group = ADBGroup;  // �������е���Դ�飬

                        // ��ȡ Prefab �� GUID
                        string assetGUID = AssetDatabase.AssetPathToGUID(path);

                        // ���� GUID �Ƿ��Ѵ�������Դ����
                        AddressableAssetEntry entry = group.GetAssetEntry(assetGUID);
                        if (entry == null)
                        {
                            // ��������ڣ��򴴽��µ���Դ��Ŀ
                            entry = settings.CreateOrMoveEntry(assetGUID, group);
                        }

                        // ��� Prefab Ϊ���޸ģ����������и���
                        AssetDatabase.SaveAssets();

                        // ɾ�� GameObject ʵ������Ϊ�����Ѿ����䱣��Ϊ Prefab
                        DestroyImmediate(newGameObject);

                        Debug.Log($"�ɹ������������µ� Prefab��{path}");
                        #endregion
                    }
                }
                else
                {
                    Debug.LogWarning($"�� {rowIndex} ��ID��ʽ�����������С�");
                }
            }

            // ���������޸�
            AssetDatabase.SaveAssets();
            Debug.Log("�����޸��ѱ��棡");
        }

        Debug.Log("Excel��ȡ��ɣ�");
        using (FileStream file = new FileStream(ExcelPath, FileMode.Create, FileAccess.Write))
        {
            workbook.Write(file); // д�뵽�ļ�
        }
    }

    /// <summary>
    /// ��Prefab�е�����д�뵽ָ����Excel�ļ���
    /// ʹ��Odin Inspector��[Button]���ԣ������ڱ༭����ͨ����ť������
    /// </summary>
    public void SetExcel()
    {
        // ���Excel·���Ƿ���Ч
        if (string.IsNullOrEmpty(ExcelPath))
        {
            Debug.LogError("Excel·��δ���ã�");
            return;
        }

        IWorkbook workbook = new XSSFWorkbook(); // ����һ���µ�.xlsx�ļ�
        ISheet sheet = workbook.CreateSheet("PrefabData"); // ����������

        // ��ȡAddressable��Դ����
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings δ�ҵ���");
            return;
        }

        int rowIndex = 0; // Excel��������

        // ������Դ�飬�ҵ����е�Prefab
        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group == null || group.entries.Count == 0) continue;

            foreach (var entry in group.entries)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.AssetPath); // ����Prefab
                if (prefab == null) continue;

                Weapon weapon = prefab.GetComponent<Weapon>(); // ��ȡPrefab�ϵ�Weapon���
                if (weapon != null)
                {
                    // ��Prefab�е�����д��Excel
                    IRow row = sheet.CreateRow(rowIndex++); // ��������
                    row.CreateCell(0).SetCellValue(prefab.name); // д��Prefab����
                   // row.CreateCell(1).SetCellValue((IRichTextString)willChangeItem._Data); // д��Weapon���������
                }
            }
        }

        // ����Excel�ļ�
        using (FileStream file = new FileStream(ExcelPath, FileMode.Create, FileAccess.Write))
        {
            workbook.Write(file); // д�뵽�ļ�
        }

        Debug.Log("Prefab������д��Excel��");
    }
}
*/