using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace WindowsFormsApplication1
{
    class Common
    {
        public static string execCMD(string[] command)
        {
            System.Diagnostics.Process pro = new System.Diagnostics.Process();
            pro.StartInfo.FileName = "cmd.exe";
            pro.StartInfo.UseShellExecute = false;      //不启用shell启动进程  
            pro.StartInfo.RedirectStandardError = true;  // 重定向错误输出 
            pro.StartInfo.RedirectStandardInput = true;  // 重定向输入    
            pro.StartInfo.RedirectStandardOutput = true;  // 重定向标准输出    
            pro.StartInfo.CreateNoWindow = true;          // 不创建新窗口    

            pro.Start();
            for (int i = 0; i < command.Length; i++)
            {
                if (!string.IsNullOrEmpty(command[i]))
                    pro.StandardInput.WriteLine(command[i]);
            }

            pro.StandardInput.WriteLine("exit");
            pro.StandardInput.AutoFlush = true;
            //获取cmd窗口的输出信息
            string output = pro.StandardError.ReadToEnd();
            if (output.Length > 1)
                output = "[error]" + output;
            output = pro.StandardOutput.ReadToEnd();
            pro.WaitForExit();//等待程序执行完退出进程
            pro.Close();
            return output;

        }
        public static List<string> GetFileList(string path)
        {
            List<string> fileList = new List<string>();

            if (Directory.Exists(path) == true)
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    fileList.Add(file);
                }

                foreach (string directory in Directory.GetDirectories(path))
                {
                    fileList.AddRange(GetFileList(directory));
                }
            }
            return fileList;
        }
        public static DataTable GetDataFromExcelByConn(string filePath)
        {
            string fileType = System.IO.Path.GetExtension(filePath);
            bool hasTitle = false;
            if (string.IsNullOrEmpty(fileType)) return null;
            try
            {
                using (DataSet ds = new DataSet())
                {
                    string strCon = string.Format("Provider=Microsoft.ACE.OLEDB.{0}.0;" +
                                    "Extended Properties=\"Excel {1}.0;HDR={2};IMEX=1;\";" +
                                    "data source={3};",
                                    (fileType == ".xls" ? 4 : 12), (fileType == ".xls" ? 8 : 12), (hasTitle ? "Yes" : "NO"), filePath);
                    OleDbConnection myConn = new OleDbConnection(strCon);
                    myConn.Open();
                    DataTable sheetsName = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" }); //得到所有sheet的名字
                    string firstSheetName = sheetsName.Rows[0][2].ToString();        //得到第一个sheet的名字
                    string strCom = string.Format("SELECT * FROM [{0}]",firstSheetName); //查询字符串
                    using (OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn))
                    {
                        myCommand.Fill(ds);
                    }
                    myConn.Close();
                    if (ds == null || ds.Tables.Count <= 0) return null;
                    return ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
            return null;
        }
        public static void MyReadTo(string path, string fileName)
        {
            DataTable result = GetDataFromExcelByConn(path);

            if (result == null)
            {
                Debug.Print("读取失败!!");
                return;
            }

            int rows = result.Rows.Count;
            int cols = result.Columns.Count;

            string[] porpertyNames = new string[cols];
            string[] porpertyType = new string[cols];
            int key=-1;
            for (int i = 0; i < cols; i++)
            {
                string[] temp = result.Rows[1][i].ToString().Split(',');
                if (!string.IsNullOrEmpty(temp[0]))
                {
                    porpertyNames[i] = temp[0];
                    porpertyType[i] = "string";
                    if (temp.Length > 1)
                        porpertyType[i] = temp[1].ToLower();
                    if (temp.Length > 2 && temp[2] == "key")
                        key = i;
                }
            }
            List<string[]> data = new List<string[]>();
            for (int i = 2; i < rows; i++)
            {
                if (string.IsNullOrEmpty(result.Rows[i][0].ToString()))
                {
                    continue;
                }

                string[] colsData = new string[cols];
                for (int j = 0; j < cols; j++)
                {
                    colsData[j] = result.Rows[i][j].ToString();
                }

                data.Add(colsData);
            }

            WriteToLua(fileName, data, porpertyNames, porpertyType,key);
        }

        private static void WriteToLua(string path, List<string[]> data, string[] porpertyNames, string[] porpertyType,int key)
        {
            string filename = path.Substring(path.LastIndexOf('\\')+1);
            filename = filename.Substring(0, filename.IndexOf("."));
            string LuaContent = "local "+filename+" = { ";
            string itemKey;
            for (int i = 0; i < data.Count; i++)
            {
                LuaContent += "\n    {";
                for (int j = 0; j < data[i].Length; j++)
                {
                    if (porpertyType[j] == null || porpertyType[j] == "")
                        continue;
                    LuaContent += "\n        " + porpertyNames[j] + " = ";
                    itemKey = data[i][j];
                    switch (porpertyType[j])
                    {
                        case "int":
                            if (string.IsNullOrEmpty(itemKey))
                            {
                                LuaContent += "nil,";
                            }
                            else
                            { 
                                LuaContent += itemKey + ",";
                            }
                            break;
                        case "float":
                            if (string.IsNullOrEmpty(itemKey))
                            {
                                LuaContent += "nil,";
                            }
                            else
                            {
                                LuaContent += itemKey + ",";
                            }
                            break;
                        case "bool":
                            if (string.IsNullOrEmpty(itemKey))
                            {
                                LuaContent += "false,";
                            }
                            else
                            {
                                LuaContent += itemKey.ToLower() + ",";
                            }
                            break;
                        case "string":
                            if (string.IsNullOrEmpty(itemKey))
                            {
                                LuaContent += "nil,";
                            }
                            else
                            {
                                itemKey = "\"" + itemKey + "\"";
                                LuaContent += itemKey  +",";
                            }
                            break;
                        case "array":
                            if (string.IsNullOrEmpty(itemKey))
                            {
                                LuaContent += "{},";
                            }
                            else
                            {

                                LuaContent += "{\"" + itemKey.Replace(",", "\",\"") + "\"},";
                            }
                            break;

                        case "arrayint":
                            if (string.IsNullOrEmpty(itemKey))
                            {
                                LuaContent += "{},";
                            }
                            else
                            {

                                LuaContent += "{" + itemKey.Replace(";", ",") + "},";
                            }
                            break;
                    }
                    if (j == key)
                    {
                        LuaContent = LuaContent.Replace("\n    {", "\n    [" + itemKey + "]={");
                    }
                }
                LuaContent += "\n    },";
            }
            LuaContent += "\n}";
            LuaContent += "\nreturn "+filename;
            try
            {
                File.WriteAllText(path, LuaContent);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            
        }
    }

}
