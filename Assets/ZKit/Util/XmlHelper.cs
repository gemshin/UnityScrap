using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System;

namespace ZKit
{
    public class XmlHandler
    {
        public enum RESULT
        {
            SUCCESS = 0,
            ERR_FILE_PATH_NOT_AVAILABLE,
            ERR_FILE_NOT_FOUND,
            ERR_FILE_NOT_AVAILABLE,
            ERR_UNKNOWN,
            END
        }

        /// <summary>
        /// Xml파일을 읽어온다.
        /// </summary>
        /// <param name="filePath">읽어오길 원하는 파일 경로+이름</param>
        /// <param name="xmlDocument">[out] 읽어온 XmlDocument</param>
        /// <returns>읽은 결과를 반환한다.</returns>
        public static RESULT LoadXML(string filePath, out XmlDocument xmlDocument)
        {
            xmlDocument = new XmlDocument();

            if (filePath == "") return RESULT.ERR_FILE_PATH_NOT_AVAILABLE;

            filePath = filePath.Trim();
            if (filePath[0] == '/') filePath = filePath.Remove(0);

            filePath = Path.ChangeExtension(filePath, ".xml");

            try
            {
                StreamReader sr = new StreamReader(filePath);
                string text = sr.ReadToEnd();
                sr.Close();
                xmlDocument.LoadXml(text);
            }
            catch (FileNotFoundException e)
            {

                Debug.Log(string.Format("{0} 파일이 없습니다.", filePath).ToXmlColorString(Color.red));
                return RESULT.ERR_FILE_NOT_FOUND;
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("{0} 파일 열기 실패.", filePath).ToXmlColorString(Color.red));
                return RESULT.ERR_UNKNOWN;
            }

            Debug.Log("File Loaded : " + filePath);
            return RESULT.SUCCESS;
        }

        /// <summary>
        /// Xml을 파일로 저장한다. 경로가 존재하면 저장, 존재하지 않으면 생성후 저장.
        /// </summary>
        /// <param name="filePath">저장하기 원하는 파일이름</param>
        /// <param name="xmlDoc">저장할 XmlDocument</param>
        public static RESULT SaveXML(string filePath, XmlDocument xmlDoc)
        {
            filePath = filePath.Trim();
            if (filePath[0] == '/') filePath = filePath.Remove(0);

            string dir = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            try
            {
                filePath = Path.ChangeExtension(filePath, ".xml");
                xmlDoc.Save(filePath);
            }
            catch (Exception)
            {
                Debug.Log(string.Format("{0}.xml 파일을 열수 없습니다.", filePath).ToXmlColorString(Color.red));
                return RESULT.ERR_UNKNOWN;
            }
            return RESULT.SUCCESS;
        }

        /// <summary>
        /// element에서 값을 찾아 Enum(T)로 반환한다.
        /// </summary>
        /// <typeparam name="T">변환하기 원하는 Enum</typeparam>
        /// <param name="element">XmlElement</param>
        /// <param name="attName">찾기를 원하는 attribute 이름</param>
        /// <returns>반환된 값</returns>
        public static T GetAttValueToEnum<T>(XmlElement element, string attName, T def)
            where T : struct, IFormattable, IConvertible, IComparable // enum
        {
            if (!element.HasAttribute(attName)) return def;
            return EnumHelper.Parse<T>(element.GetAttribute(attName));
        }

        /// <summary>
        /// element에서 attName에 해당하는 값을 찾아서 ret로 반환한다.
        /// </summary>
        /// <param name="element">XmlElement</param>
        /// <param name="attName">찾기를 원하는 attribute 이름</param>
        /// <param name="ret">[out] 반환 받을 변수</param>
        /// <param name="def">찾는 값이 없을 경우 ret에 들어갈 값. 초기값은 ""</param>
        public static void GetAttValue(XmlElement element, string attName, out string ret, string def = "")
        {
            ret = element.GetAttribute(attName);
            if (ret == "") ret = def;
        }

        /// <summary>
        /// element에서 attName에 해당하는 값을 찾아서 ret로 반환한다.
        /// </summary>
        /// <param name="element">XmlElement</param>
        /// <param name="attName">찾기를 원하는 attribute 이름</param>
        /// <param name="ret">[out] 반환 받을 변수</param>
        /// <param name="def">찾는 값이 없을 경우 ret에 들어갈 값. 초기값은 0</param>
        /// <returns>해당하는 값이 있으면 true, 없으면 false를 반환</returns>
        public static bool GetAttValue(XmlElement element, string attName, out int ret, int def = 0)
        {
            XmlNode node = element.Attributes.GetNamedItem(attName);
            ret = def;

            if (node == null) return false;

            ret = int.Parse(node.Value);
            return true;
        }

        /// <summary>
        /// element에서 attName에 해당하는 값을 찾아서 ret로 반환한다.
        /// </summary>
        /// <param name="element">XmlElement</param>
        /// <param name="attName">찾기를 원하는 attribute 이름</param>
        /// <param name="ret">[out] 반환 받을 변수</param>
        /// <param name="def">찾는 값이 없을 경우 ret에 들어갈 값. 초기값은 0</param>
        /// <returns>해당하는 값이 있으면 true, 없으면 false를 반환</returns>
        public static bool GetAttValue(XmlElement element, string attName, out float ret, float def = 0f)
        {
            XmlNode node = element.Attributes.GetNamedItem(attName);
            ret = def;

            if (node == null) return false;

            ret = float.Parse(node.Value);
            return true;
        }

        /// <summary>
        /// element에서 attName에 해당하는 값을 찾아서 ret로 반환한다.
        /// </summary>
        /// <param name="element">XmlElement</param>
        /// <param name="attName">찾기를 원하는 attribute 이름</param>
        /// <param name="ret">[out] 반환 받을 변수. 값이 없으면 null 반환</param>
        /// <returns>해당하는 값이 있으면 true, 없으면 false를 반환</returns>
        public static bool GetAttValue(XmlElement element, string attName, out float? ret)
        {
            XmlNode node = element.Attributes.GetNamedItem(attName);

            if (node == null) { ret = null; return false; }

            ret = float.Parse(node.Value);
            return true;
        }

        /// <summary>
        /// element에서 attName에 해당하는 값을 찾아서 ret로 반환한다.
        /// </summary>
        /// <param name="element">XmlElement</param>
        /// <param name="attName">찾기를 원하는 attribute 이름</param>
        /// <param name="ret">[out] 반환 받을 변수</param>
        /// <param name="def">찾는 값이 없을 경우 ret에 들어갈 값. 초기값은 false</param>
        /// <returns>해당하는 값이 있으면 true, 없으면 false를 반환</returns>
        public static bool GetAttValue(XmlElement element, string attName, out bool ret, bool def = false)
        {
            XmlNode node = element.Attributes.GetNamedItem(attName);
            ret = def;

            if (node == null) return false;

            ret = bool.Parse(node.Value);
            return true;
        }

        /// <summary>
        /// element에서 attName에 해당하는 값을 찾아서 ret로 반환한다. 
        /// </summary>
        /// <param name="element">XmlElement</param>
        /// <param name="attName">찾기를 원하는 attribute 이름.</param>
        /// <param name="ret">[out] 반환 받을 변</param>
        /// <param name="def">찾는 값이 없을 경우 ret에 들어갈 값.</param>
        /// <returns>값이 있으면 true, 없으면 false를 반환</returns>
        public static bool GetAttValue(XmlElement element, string attName, out Point ret, Point def)
        {
            string tmp = element.GetAttribute(attName);
            if (tmp == "")
            {
                ret = def;
                return false;
            }
            ret = new Point();
            ret.FromString(tmp);

            return true;
        }

        /// <summary>
        /// element에서 attName에 해당하는 값을 찾아서 ret로 반환한다. 
        /// </summary>
        /// <param name="element">XmlElement</param>
        /// <param name="attName">찾기를 원하는 attribute 이름.</param>
        /// <param name="ret">[out] 반환 받을 변수</param>
        /// <param name="def">찾는 값이 없을 경우 ret에 들어갈 값.</param>
        /// <returns>하나라도 값이 있으면 true, 모두 없으면 false를 반환</returns>
        public static bool GetAttValue(XmlElement element, string attName, out UnityEngine.Vector3 ret, UnityEngine.Vector3 def)
        {
            string tmp = element.GetAttribute(attName);
            if (tmp == "")
            {
                ret = def;
                return false;
            }

            ret = new Vector3();
            ret.FromString(ref ret, tmp);

            return true;
        }

        /// <summary>
        /// element에서 attName에 해당하는 값을 찾아서 ret로 반환한다. 
        /// </summary>
        /// <param name="element">XmlElement</param>
        /// <param name="attName">찾기를 원하는 attribute 이름. ex) "position" 이름이면 값들은 "positionx", "positiony", "positionz" 입니다.</param>
        /// <param name="ret">[out] 반환 받을 변수</param>
        /// <param name="def">찾는 값이 없을 경우 ret에 들어갈 값.</param>
        /// <returns>하나라도 값이 있으면 true, 모두 없으면 false를 반환</returns>
        public static bool GetSplitAttValue(XmlElement element, string attName, out UnityEngine.Vector3 ret, UnityEngine.Vector3 def)
        {
            XmlNode nodeX = element.Attributes.GetNamedItem(attName + "x");
            XmlNode nodeY = element.Attributes.GetNamedItem(attName + "y");
            XmlNode nodeZ = element.Attributes.GetNamedItem(attName + "z");
            ret = def;

            if (nodeX != null) ret.x = float.Parse(nodeX.Value);
            if (nodeY != null) ret.y = float.Parse(nodeY.Value);
            if (nodeZ != null) ret.z = float.Parse(nodeZ.Value);

            if (nodeX == null && nodeY == null && nodeZ == null) return false;

            return true;
        }

    }
}
