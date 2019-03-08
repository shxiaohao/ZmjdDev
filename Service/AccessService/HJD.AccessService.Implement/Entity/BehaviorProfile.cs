using HJD.AccessService.Contract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HJD.AccessService.Implement.Entity
{
    public class BehaviorProfile
    {
        private static readonly BehaviorProfile behaviorProfile = new BehaviorProfile();
        private string Path = System.AppDomain.CurrentDomain.BaseDirectory + System.Configuration.ConfigurationManager.AppSettings["BehaviorProfilePath"];
        private List<BehaviorField> BehaviorFieldList = null;

        /// <summary>
        /// 类实例
        /// </summary>
        /// <returns></returns>
        public static BehaviorProfile GetInstance()
        {
            return behaviorProfile;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        private BehaviorProfile()
        {
            if (BehaviorFieldList == null)
            {
                GetBehaviorProfile();
            }
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        private void GetBehaviorProfile()
        {
            BehaviorFieldList = new List<BehaviorField>();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Path);
                XmlElement root = doc.DocumentElement;
                if (root != null && root.Name.ToLower() == "behaviorsettings")
                {
                    for (int loop = 0; loop < root.ChildNodes.Count; loop++)
                    {
                        XmlNode indexSetNode = root.ChildNodes[loop];
                        if (indexSetNode.Name.ToLower() == "fieldsettings")
                        {
                            for (int i = 0; i < indexSetNode.ChildNodes.Count; i++)
                            {
                                XmlNode fieldNode = indexSetNode.ChildNodes[i];
                                if (fieldNode.Name.ToLower() == "field")
                                {
                                    BehaviorField bField = new BehaviorField();
                                    for (int j = 0; j < fieldNode.Attributes.Count; j++)
                                    {
                                        var attributesValue = fieldNode.Attributes[j].Value;

                                        switch (fieldNode.Attributes[j].Name)
                                        {
                                            case "Name": bField.Name = attributesValue; break;
                                            case "Page": bField.Page = attributesValue; break;
                                            case "Element": bField.Element = attributesValue; break;
                                            case "Event": bField.Event = attributesValue; break;
                                            case "Enable": bField.Enable = attributesValue; break;
                                        }

                                        //PropertyInfo field = bField.GetType().GetProperty(fieldNode.Attributes[j].Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                        //if (field == null)
                                        //{
                                        //    continue;
                                        //}
                                        
                                        //if (field.PropertyType == typeof(Int32))
                                        //{
                                        //    field.SetValue(bField, Int32.Parse(attributesValue), null);
                                        //}
                                        //else
                                        //{
                                        //    field.SetValue(bField, attributesValue, null);
                                        //}
                                    }
                                    BehaviorFieldList.Add(bField);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 获取字段属性。
        /// </summary>
        public BehaviorField GetBehaviorField(string strFieldName)
        {
            BehaviorField fieldProperty = null;

            for (int i = 0; i < BehaviorFieldList.Count; i++)
            {
                if (BehaviorFieldList[i].Name.ToLower() == strFieldName.ToLower())
                {
                    fieldProperty = BehaviorFieldList[i];
                    break;
                }
            }

            return fieldProperty;
        }
    }
}
