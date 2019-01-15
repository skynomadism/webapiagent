using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiAgent
{
    public class LogFile
    {
        private string _FilePath = "";
        /// <summary>
        /// 不定的某信文件的完整路径
        /// </summary>
        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        }
        private string _FileName = "";
        /// <summary>
        /// 在当前程序目录下的文件
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }
        /// <summary>
        /// 获取一个文本文件的内容
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string GetText(string filepath)
        {
            using (StreamReader reader = new StreamReader(filepath))
            {
                //string strLine = reader.ReadLine();
                return reader.ReadToEnd();
            }
        }
        /// <summary>
        /// 向指定的文本文件里写入信息
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="myStr"></param>
        public static void WriteText(string filepath, string myStr)
        {
            using (StreamWriter writer = new StreamWriter(filepath))
            {
                writer.WriteLine(myStr);
            }
        }
        /// <summary>
        /// 将指定信息写入程序日志（当前程序目示下的LOG文件夹内）（本函数只支持应用程序，不支持web程序）
        /// </summary>
        /// <param name="sMsg"></param>
        public static void WriteLog(string sMsg)
        {
            if (sMsg != "")
            {
                WriteLog(sMsg, "Default", 1, 15);
            }
        }
        /// <summary>
        /// 将指定信息写入程序日志（当前程序目示下的LOG文件夹内）（本函数只支持应用程序，不支持web程序）
        /// </summary>
        /// <param name="sMsg">需要写入日志的信息</param>
        /// <param name="LogTypeName">日志文件所在的文件夹</param>
        /// <param name="Interval">0表示按小时分日志文件，1表示按天分日志文件，2表示按10分钟为单位分日志文件</param>
        /// <param name="KeepLogDaysCount">自动保留日志的天数(按小时来分日志的话，可以进行正确清理)</param>
        public static void WriteLog(string sMsg, string LogTypeName, int Interval = 0, int KeepLogDaysCount = 3)
        {
            try
            {
                if (sMsg != "")
                {
                    #region 按时间构造日志文件名称
                    //Random randObj = new Random(DateTime.Now.Millisecond);
                    //int file = randObj.Next() + 1;
                    string time = "";
                    if (Interval == 0) //按时分
                    {
                        time = DateTime.Now.ToString("yyyyMMddHHmm").Substring(0, 10);
                    }
                    if (Interval == 1) //按天分
                    {
                        time = DateTime.Now.ToString("yyyyMMddHHmm").Substring(0, 8);
                    }
                    if (Interval == 2) //按十分钟分
                    {
                        time = DateTime.Now.ToString("yyyyMMddHHmm").Substring(0, 11);
                    }
                    if (time == "")
                    {
                        time = DateTime.Now.ToString("yyyyMMddHHmm").Substring(0, 10);
                    }
                    string filename = time + ".log";
                    #endregion

                    try
                    {
                        string FileDir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Sky_log\\" + LogTypeName + "\\";
                        if (!Directory.Exists(FileDir))
                        { Directory.CreateDirectory(FileDir); }
                        FileInfo fi = new FileInfo(FileDir + filename);
                        if (!fi.Exists)
                        {
                            using (StreamWriter sw = fi.CreateText())
                            {
                                sw.WriteLine(DateTime.Now + "\n" + sMsg + "\n");
                                sw.Close();
                            }
                        }
                        else
                        {
                            using (StreamWriter sw = fi.AppendText())
                            {
                                sw.WriteLine(DateTime.Now + "\n" + sMsg + "\n");
                                sw.Close();
                            }
                        }


                        #region 自动清理日志
                        //用连续清三次的办法保证日志一定会被清除。这段代码是为了保证程序在停止三天时间内任一时间启动都可以保证正常清理不需要的日志
                        string OldFileName = FileDir + DateTime.Now.AddDays(-KeepLogDaysCount).ToString("yyyyMMddHH") + ".log";

                        if (File.Exists(OldFileName))
                        { File.Delete(OldFileName); }

                        OldFileName = FileDir + DateTime.Now.AddDays(-(KeepLogDaysCount + 1)).ToString("yyyyMMddHH") + ".log";

                        if (File.Exists(OldFileName))
                        { File.Delete(OldFileName); }

                        OldFileName = FileDir + DateTime.Now.AddDays(-(KeepLogDaysCount + 2)).ToString("yyyyMMddHH") + ".log";

                        if (File.Exists(OldFileName))
                        { File.Delete(OldFileName); }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        
                    }

                }
            }
            catch { }
        }

        /// <summary>
        /// 向控制台输出一段彩色文字
        /// 并同时保存成日志文件(日志文件会依据MessageType写同名文件夹内)
        /// </summary>
        /// <param name="sMsg"></param>
        /// <param name="MessageType"></param>
        /// <param name="TxtColor"></param>
        public static void RichConsole(string sMsg, string MessageType="Default",ConsoleColor TxtColor= ConsoleColor.White,bool isAddTime=true) 
        {
            Console.ForegroundColor = TxtColor;
            Console.WriteLine(isAddTime?( DateTime.Now.ToString("HH:mm:ss")+" "+sMsg):sMsg);
            Console.ResetColor();
            if (MessageType == "Default")
            {
                //WriteLog(sMsg);
            }
            else 
            {
                WriteLog(sMsg, MessageType, 0, 7);
            }
        }
        /// <summary>
        /// 向控制台输出一段彩色文字
        /// 并同时保存成日志文件（日志文件会写入default文件夹）
        /// </summary>
        /// <param name="sMsg"></param>
        /// <param name="TxtColor"></param>
        public static void RichConsole(string sMsg, ConsoleColor TxtColor,bool isAddTime=true) 
        {
            RichConsole(sMsg, "Default", TxtColor,isAddTime);
        }
        /// <summary>
        /// 向控制台输出一段彩色文字
        /// 并同时保存成日志文件（日志文件会写入MessageType同名的文件夹）
        /// </summary>
        /// <param name="sMsg"></param>
        /// <param name="MessageType"></param>
        public static void RichConsole(string sMsg, string MessageType)
        {
            RichConsole(sMsg, MessageType, ConsoleColor.White);
        }
        public static string ConvertToHexString(byte[] buffer)
        {
            string msg = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                msg += ("[" + i.ToString("00") + "]" + "0x" + buffer[i].ToString("X2") + (i == buffer.Length - 1 ? "" : ","));
                if ((i + 1) % 16 == 0) { msg += Environment.NewLine; }
            }
            return msg;
        }
        /// <summary>
        /// 将一段文字写入到属性指定的文件中
        /// </summary>
        /// <param name="dataText"></param>
        /// <returns></returns>
        public bool WriteLine(string dataText)
        {

            FileStream fs = null;
            StreamWriter sw = null;
            bool ret = true;
            try
            {
                string FileName = _FilePath;
                //CHECK文件目录存在不   
                if (!Directory.Exists(FileName))
                {
                    Directory.CreateDirectory(FileName);
                }
                FileName += @"\" + _FileName;
                //CHECK文件存在不   
                if (!File.Exists(FileName))
                {
                    FileStream tempfs = File.Create(FileName);
                    tempfs.Close();
                }
                fs = new FileStream(
                    FileName,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.None);
                fs.Seek(0, System.IO.SeekOrigin.End);
                sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.WriteLine(dataText);
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
            catch (Exception)
            {
                ret = false;
            }
            finally
            {
                try
                {
                    if (sw != null)
                    {
                        sw.Close();
                        sw = null;
                    }
                    if (fs != null)
                    {
                        fs.Close();
                        fs = null;
                    }
                }
                catch
                {
                }
            }
            return ret;
        }
        /// <summary>
        ///  获取指定文件夹的大小 
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public long countsize(System.IO.DirectoryInfo dir)
        {
            long size = 0;
            FileInfo[] files = dir.GetFiles();
            foreach (System.IO.FileInfo info in files)
            {
                size += info.Length;
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo dirinfo in dirs)
            {
                size += countsize(dirinfo);
            }
            return size;
        }   
    }

}
