/*using NPOI.XSSF.UserModel; // 用于操作Excel文件（.xlsx）
using NPOI.SS.UserModel;   // 提供Excel表格的通用接口
using System.IO;           // 文件操作相关的命名空间
using UnityEngine;         // Unity的核心命名空间
using UnityEditor;         // 用于编辑器功能的命名空间
using UnityEditor.AddressableAssets.Settings; // 用于操作Addressable资源的设置
using UnityEditor.AddressableAssets;          // Addressable资源管理相关
using Sirenix.OdinInspector;                  // Odin Inspector插件，用于增强编辑器功能
using System.Collections.Generic;
using System;

/// <summary>
/// 通过NPOI实现Excel和Unity Prefab数据的读写操作的管理器。
/// 支持读取Excel更新Prefab和将Prefab数据写入Excel。
/// </summary>
public class ExcelManager : MonoBehaviour
{
    /// <summary>
    /// Excel文件路径，由用户通过Inspector设置。
    /// </summary>
    public string ExcelPath;
    public string NewPrefabPath = "NewPrefabPath";


    
    /// <summary>
    /// Addressable资源组引用，用于操作其中的Prefab。
    /// </summary>
    public AddressableAssetGroup ADBGroup;

    /// <summary>
    /// 从Excel读取数据并更新Prefab中的组件数据。
    /// 使用Odin Inspector的[Button]特性，允许在编辑器中通过按钮触发。
    /// </summary>
    /// 
    [Button]
    #region 读取Excel数据并更新Prefabs
    /// <summary>
    /// 读取指定的Excel工作表，并根据Excel数据更新Prefab属性，
    /// 如果Prefab不存在，则创建新的Prefab。最终保存Excel文件。
    /// </summary>
    /// <param name="OpenExcelSheetName">要打开的工作表名称，默认为 "WeaponData"。</param>
    public void ReadExcel(string OpenExcelSheetName = "WeaponData")
    {
        // 验证Excel路径是否有效，如果无效则退出
        if (!ValidateExcelPath()) return;

        // 加载Excel工作簿
        IWorkbook workbook = LoadWorkbook(ExcelPath);
        if (workbook == null) return;  // 如果加载失败，退出

        // 加载指定名称的工作表
        ISheet sheet = LoadSheet(workbook, OpenExcelSheetName);
        if (sheet == null) return;  // 如果工作表加载失败，退出

        // 构建Prefab字典，使用 ID 作为键
        Dictionary<int, GameObject> prefabDictionary = BuildPrefabDictionary();

        // 遍历Excel数据，根据数据更新Prefab或创建新的Prefab
        UpdateOrCreatePrefabs(sheet, prefabDictionary, OpenExcelSheetName);

        // 保存修改后的Excel工作簿
        SaveWorkbook(workbook, ExcelPath);

        Debug.Log("Excel读取完成！");
    }
    #endregion


    #region 更新Prefab属性
    /// <summary>
    /// 根据Excel数据更新Prefab的属性。
    /// </summary>
    /// <param name="row">Excel行数据。</param>
    /// <param name="prefab">需要更新的Prefab。</param>
    /// <param name="sheetName">当前工作表名称，用于区分数据类型。</param>
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
            Debug.Log($"Prefab ID: {willChangeItem.GetData().ID} 更新成功！");
        }
    }
    #endregion

    #region 验证Excel路径
    /// <summary>
    /// 验证Excel路径是否已正确设置。
    /// </summary>
    /// <returns>路径有效返回true，无效返回false。</returns>
    private bool ValidateExcelPath()
    {
        if (string.IsNullOrEmpty(ExcelPath))
        {
            Debug.LogError("Excel路径未设置！");
            return false;
        }
        return true;
    }
    #endregion

    #region 更新或创建Prefabs
    /// <summary>
    /// 遍历Excel中的数据，根据ID更新对应的Prefab属性，
    /// 如果ID不存在，则创建新的Prefab。
    /// </summary>
    /// <param name="sheet">Excel工作表对象。</param>
    /// <param name="prefabDictionary">现有Prefab字典。</param>
    /// <param name="sheetName">当前工作表名称。</param>
    private void UpdateOrCreatePrefabs(ISheet sheet, Dictionary<int, GameObject> prefabDictionary, string sheetName)
    {
        for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
        {
            IRow row = sheet.GetRow(rowIndex);
            if (row == null) continue;

            // 忽略标题行
            if (rowIndex == 0) continue;

            // 获取Prefab的ID
            string prefabIdString = row.GetCell(0)?.ToString().Trim();
            if (int.TryParse(prefabIdString, out int prefabId))
            {
                if (prefabDictionary.TryGetValue(prefabId, out GameObject prefab))
                {
                    // 更新现有Prefab
                    UpdatePrefabProperties(row, prefab, sheetName);
                }
                else
                {
                    // 创建新Prefab
                    CreateNewPrefab(row, prefabId, sheetName);
                }
            }
            else
            {
                Debug.LogWarning($"行 {rowIndex} 的ID格式错误，跳过该行。");
            }
        }
    }
    #endregion

    #region 保存Excel工作簿
    /// <summary>
    /// 将修改后的Excel工作簿保存到指定路径。
    /// </summary>
    /// <param name="workbook">需要保存的工作簿对象。</param>
    /// <param name="path">保存路径。</param>
    private void SaveWorkbook(IWorkbook workbook, string path)
    {
        try
        {
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(file);
                Debug.Log("Excel文件已成功保存！");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"保存Excel文件失败：{e.Message}");
        }
    }
    #endregion

    #region 创建新Prefab
    /// <summary>
    /// 创建一个新的Prefab并设置其初始属性。
    /// </summary>
    /// <param name="row">Excel行数据。</param>
    /// <param name="prefabId">Prefab的唯一ID。</param>
    /// <param name="sheetName">当前工作表名称。</param>
    private void CreateNewPrefab(IRow row, int prefabId, string sheetName)
    {
        GameObject newGameObject = new GameObject();
        Weapon weapon = newGameObject.AddComponent<Weapon>();

        // 初始化数据
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

        // 保存Prefab
        string path = AssetDatabase.GenerateUniqueAssetPath(NewPrefabPath + weapon._Data.Name + ".prefab");
        PrefabUtility.SaveAsPrefabAsset(newGameObject, path);

        Debug.Log($"成功创建新Prefab: {path}");
        DestroyImmediate(newGameObject);
    }
    #endregion

    #region 加载Excel文件
    /// <summary>
    /// 加载Excel文件并返回工作簿对象。
    /// </summary>
    /// <param name="path">Excel文件路径。</param>
    /// <returns>加载成功返回工作簿对象，否则返回null。</returns>
    private IWorkbook LoadWorkbook(string path)
    {
        try
        {
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Debug.Log("Excel文件成功加载！");
                return new XSSFWorkbook(file); // 打开.xlsx文件
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载Excel文件失败：{e.Message}");
            return null;
        }
    }
    #endregion

    #region 加载工作表
    /// <summary>
    /// 根据名称加载指定的工作表。
    /// </summary>
    /// <param name="workbook">Excel工作簿对象。</param>
    /// <param name="sheetName">工作表名称。</param>
    /// <returns>加载成功返回工作表对象，否则返回null。</returns>
    private ISheet LoadSheet(IWorkbook workbook, string sheetName)
    {
        ISheet sheet = sheetName != "null" ? workbook.GetSheet(sheetName) : workbook.GetSheetAt(0);
        if (sheet == null)
        {
            Debug.LogError($"未找到名为 {sheetName} 的工作表！");
            return null;
        }
        Debug.Log($"成功加载工作表: {sheetName}");
        return sheet;
    }
    #endregion

    #region 构建Prefab字典
    /// <summary>
    /// 构建Prefab字典，使用Prefab的 ID 作为键。
    /// </summary>
    /// <returns>返回包含Prefab的字典。</returns>
    private Dictionary<int, GameObject> BuildPrefabDictionary()
    {
        Dictionary<int, GameObject> prefabDictionary = new Dictionary<int, GameObject>();

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings 未找到！");
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
                    Debug.Log($"成功将Prefab ID:{weapon._Data.ID} 加入字典");
                }
            }
        }

        return prefabDictionary;
    }
    #endregion




    [Button]
    public void ReadExcelByStringKey(string OpenExcelSheetName = "WeaponData")
    {
        // 创建一个字典用于存储Prefab，以 `willChangeItem.GetData().ID` 作为键
        Dictionary<int, GameObject> prefabDictionary = new Dictionary<int, GameObject>();
        List<string> ExcelFirstRow = new List<string>();//按顺序保存Excel第一行数据

        // 检查Excel路径是否有效
        if (string.IsNullOrEmpty(ExcelPath))
        {
            Debug.LogError("Excel路径未设置！");
            return;
        }

        IWorkbook workbook;

        // 读取Excel文件并解析内容
        using (FileStream file = new FileStream(ExcelPath, FileMode.Open, FileAccess.Read))
        {
            workbook = new XSSFWorkbook(file); // 打开.xlsx文件
            Debug.Log("Excel文件成功加载！");

            // 如果指定了工作表名称，则获取对应的工作表；否则默认获取第一个工作表
            ISheet sheet = OpenExcelSheetName != "null" ? workbook.GetSheet(OpenExcelSheetName) : workbook.GetSheetAt(0);
            if (sheet == null)
            {
                Debug.LogError($"未找到名为 {OpenExcelSheetName} 的工作表！");
                return;
            }

            Debug.Log($"成功加载工作表: {OpenExcelSheetName}");

            // 获取Addressable资源设置
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("AddressableAssetSettings 未找到！");
                return;
            }

            // 遍历资源组，将Prefab存入字典
            Debug.Log("开始遍历Addressable资源组...");
            foreach (AddressableAssetGroup group in settings.groups)
            {
                if (group == null || group.entries.Count == 0) continue;

                foreach (var entry in group.entries)
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.AssetPath); // 加载Prefab
                    if (prefab == null) continue;

                    Weapon weapon = prefab.GetComponent<Weapon>(); // 获取Prefab上的Weapon组件
                    if (weapon != null)
                    {
                        // 将Prefab的 `ID` 作为键存入字典
                        if (weapon.GetData() != null)
                        {
                            prefabDictionary[weapon.GetData().ID] = prefab;
                            Debug.Log($"成功将Prefab ID:{weapon.GetData().ID} 加入字典");
                        }
                    }
                }
            }

            // 遍历Excel中的数据并更新对应的Prefab
            Debug.Log("开始读取Excel数据并更新Prefab...");
            for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row == null) continue;


                // TODO 填充 ExcelFirstRow
                // 忽略标题行
                if (rowIndex == 0)
                {
                    Debug.Log("跳过标题行...");
                    continue;
                }

                // 获取Prefab的 `ID`（假设在Excel的第一列）
                string prefabIdString = row.GetCell(0)?.ToString().Trim(); // 去除多余的空格
                if (int.TryParse(prefabIdString, out int prefabId))
                {
                    Debug.Log($"处理Prefab ID: {prefabId}");

                    // 如果字典中存在对应的Prefab，则更新其属性
                    if (prefabDictionary.TryGetValue(prefabId, out GameObject prefab))
                    {
                        Weapon weapon = prefab.GetComponent<Weapon>();
                        if (weapon != null && weapon.GetData() != null)
                        {

                            //名字
                            weapon.GetData().Name = row.GetCell(1)?.ToString();

                            //描述
                            weapon.GetData().Description = row.GetCell(2)?.ToString();

                            //预制件路径
                            if (row.GetCell(3)?.ToString() == null)
                            {
                                //获取 willChangeItem.gameObject预制件路径
                                weapon.GetData().PrefabPath = AssetDatabase.GetAssetPath(weapon.gameObject);
                                Debug.Log("Prefab路径为空，已自动获取预制件路径！" + weapon.GetData().PrefabPath);
                                row.CreateCell(3).SetCellValue(weapon._Data.PrefabPath);
                                //保存excel

                            }
                            else
                            {
                                weapon._Data.PrefabPath = row.GetCell(3)?.ToString();
                                Debug.Log("Prefab路径已更新！" + weapon._Data.PrefabPath);
                            }

                            //物体体积
                            weapon._Data.Volume = float.Parse(row.GetCell(4)?.ToString());

                            //耐久度
                            weapon._Data.Durability = float.Parse(row.GetCell(5)?.ToString());


                            if (OpenExcelSheetName == "WeaponData")
                            {
                                //伤害
                                weapon._Data._damage = float.Parse(row.GetCell(6)?.ToString());
                                //精力消耗速度
                                weapon._Data.StaminaCost = float.Parse(row.GetCell(7)?.ToString());
                                //最大攻击距离
                                weapon._Data.MaxAttackDistance = float.Parse(row.GetCell(8)?.ToString());
                                //物体攻击速度
                                weapon._Data.AttackSpeed = float.Parse(row.GetCell(9)?.ToString());
                                //物体返回速度
                                weapon._Data.ReturnSpeed = float.Parse(row.GetCell(10)?.ToString());
                            }





                            // 标记Prefab为已修改
                            EditorUtility.SetDirty(prefab);
                            Debug.Log($"Prefab ID: {prefabId} 更新成功！");


                        }
                    }
                    else
                    {
                        Debug.LogWarning($"未找到Prefab ID为 {prefabId} 的Prefab！");
                        #region 创建新Prefab

                        // 创建一个新的 GameObject 并为其添加 Weapon 组件
                        GameObject newGameObject = new GameObject();  // 创建新的 GameObject
                        Weapon weapon = newGameObject.AddComponent<Weapon>();  // 为 GameObject 添加 Weapon 组件
                        GameObject newGameObject_Sprite = new GameObject();  // 创建新的 GameObject
                        newGameObject_Sprite.AddComponent<SpriteRenderer>();  // 为 GameObject 添加 SpriteRenderer 组件
                        newGameObject_Sprite.transform.SetParent(newGameObject.transform);  // 设置父物体
                        newGameObject_Sprite.transform.localPosition = Vector3.zero;  // 设置位置
                        newGameObject_Sprite.transform.localScale = Vector3.one;  // 设置缩放
                        #endregion

                        #region 配置 Data 组件

                        // 初始化 WeaponData 组件
                     *//*   weapon._Data = new WeaponData();  // 初始化数据*//*
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

                        #region 保存Prefab
                        // 生成 Prefab 并保存到指定路径
                        string path = AssetDatabase.GenerateUniqueAssetPath(NewPrefabPath + weapon.gameObject.name + ".prefab");
                        PrefabUtility.SaveAsPrefabAsset(newGameObject, path);  // 将 GameObject 保存为 Prefab

                        // TODO 将 Prefab 加入 Addressable 资源组
                        AddressableAssetSettings ADBSettings = AddressableAssetSettingsDefaultObject.Settings;
                        AddressableAssetGroup group = ADBGroup;  // 查找已有的资源组，

                        // 获取 Prefab 的 GUID
                        string assetGUID = AssetDatabase.AssetPathToGUID(path);

                        // 检查该 GUID 是否已存在于资源组中
                        AddressableAssetEntry entry = group.GetAssetEntry(assetGUID);
                        if (entry == null)
                        {
                            // 如果不存在，则创建新的资源条目
                            entry = settings.CreateOrMoveEntry(assetGUID, group);
                        }

                        // 标记 Prefab 为已修改，并保存所有更改
                        AssetDatabase.SaveAssets();

                        // 删除 GameObject 实例，因为我们已经将其保存为 Prefab
                        DestroyImmediate(newGameObject);

                        Debug.Log($"成功创建并保存新的 Prefab：{path}");
                        #endregion
                    }
                }
                else
                {
                    Debug.LogWarning($"行 {rowIndex} 的ID格式错误，跳过该行。");
                }
            }

            // 保存所有修改
            AssetDatabase.SaveAssets();
            Debug.Log("所有修改已保存！");
        }

        Debug.Log("Excel读取完成！");
        using (FileStream file = new FileStream(ExcelPath, FileMode.Create, FileAccess.Write))
        {
            workbook.Write(file); // 写入到文件
        }
    }

    /// <summary>
    /// 将Prefab中的数据写入到指定的Excel文件。
    /// 使用Odin Inspector的[Button]特性，允许在编辑器中通过按钮触发。
    /// </summary>
    public void SetExcel()
    {
        // 检查Excel路径是否有效
        if (string.IsNullOrEmpty(ExcelPath))
        {
            Debug.LogError("Excel路径未设置！");
            return;
        }

        IWorkbook workbook = new XSSFWorkbook(); // 创建一个新的.xlsx文件
        ISheet sheet = workbook.CreateSheet("PrefabData"); // 创建工作表

        // 获取Addressable资源设置
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings 未找到！");
            return;
        }

        int rowIndex = 0; // Excel的行索引

        // 遍历资源组，找到其中的Prefab
        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group == null || group.entries.Count == 0) continue;

            foreach (var entry in group.entries)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(entry.AssetPath); // 加载Prefab
                if (prefab == null) continue;

                Weapon weapon = prefab.GetComponent<Weapon>(); // 获取Prefab上的Weapon组件
                if (weapon != null)
                {
                    // 将Prefab中的数据写入Excel
                    IRow row = sheet.CreateRow(rowIndex++); // 创建新行
                    row.CreateCell(0).SetCellValue(prefab.name); // 写入Prefab名称
                   // row.CreateCell(1).SetCellValue((IRichTextString)willChangeItem._Data); // 写入Weapon组件的数据
                }
            }
        }

        // 保存Excel文件
        using (FileStream file = new FileStream(ExcelPath, FileMode.Create, FileAccess.Write))
        {
            workbook.Write(file); // 写入到文件
        }

        Debug.Log("Prefab数据已写入Excel！");
    }
}
*/