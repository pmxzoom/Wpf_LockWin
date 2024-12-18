using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Windows;
using System.Data.Common;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Wpf.LockWindow
{
    public static class ImportImageHelper
    {
        //上传二进制流数据到数据库
        public static void Updata_SQL(SQLiteConnection conn,string FileName)
        {
            try
            {
                byte[] picData = GetFileBytes(FileName);
                FileName = "\'" + Path.GetFileName(FileName) + "\'";
                string sql = "";
                //conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                // 直接返这个值放到数据就行了           
                sql = $"Insert into ImageTable (Id,ImageData,ImageName) Values (null,@Data, {FileName})";
                cmd.CommandText = sql;
                cmd.Parameters.Add("@Data", DbType.Object, picData.Length);
                cmd.Parameters["@Data"].Value = picData;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常");
            }


        }

        //直接上传图片  内部自动转换为二进制流数据
        public static void Updata_SQL(SQLiteConnection conn,string FileName, Image Picture)
        {
            try
            {
                byte[] picData = ImageToByte(Picture);
                string sql = "";
                //conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                // 直接返这个值放到数据就行了           
                sql = $"Insert into ImageTable (ImageData,ImageName) Values (@Data, {FileName})";
                cmd.CommandText = sql;
                cmd.Parameters.Add("@Data", DbType.Object, picData.Length);
                cmd.Parameters["@Data"].Value = picData;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常");
            }

        }




        //将图片数据转换为二进制流数据
        private static byte[] ImageToByte(Image Picture)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                if (Picture == null)
                    return new byte[ms.Length];
                Picture.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] BPicture = new byte[ms.Length];
                BPicture = ms.GetBuffer();
                return BPicture;
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message, "异常");
                return null;
            }

        }

        //二进制流转为图片方法
        public static Stream Byte_Image(object value)
        {
            try
            {
                byte[] picData = (byte[])value;
                MemoryStream ms = new MemoryStream(picData);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                Stream stream = ms;
                //StreamToFile(ms, "C:\\Users\\EDY\\Downloads\\【哲风壁纸】金克斯-雨中女.png");
                return stream;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常");
                return null;
            }

        }

        //转文件
        public static void StreamToFile(Stream stream, string fileName)
        {
            // 把 Stream 转换成 byte[]
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            // 把 byte[] 写入文件
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }

        //将文件读取转换为二进制流文件
        public static byte[] GetFileBytes(string Filename)
        {
            try
            {
                if (Filename == "")
                    return null;
                FileStream fileStream = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                byte[] fileBytes = binaryReader.ReadBytes((int)fileStream.Length);
                binaryReader.Close();
                return fileBytes;
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message, "异常");
                return null;
            }
        }

    }
}
