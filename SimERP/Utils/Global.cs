
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Data.Odbc;
using System.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using Remotion.Linq.Clauses;

namespace SimERP.Utils
{
    public static class Global
    {
        public static DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }

                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader
                        ? firstRowCell.Text
                        : string.Format("Column {0}", firstRowCell.Start.Column));
                }

                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }

                return tbl;
            }
        }

        public static DataTable ProcessFile(string path, string dataFileNameCIFPrice, string strTableName, ref string messageText)
        {
            DataTable dtRsl = new DataTable();
            var file = Directory.GetFiles(path, dataFileNameCIFPrice).FirstOrDefault();
            if (File.Exists(file))
            {
                string connetionString = null;
                connetionString = "Driver={Microsoft Access Driver (*.mdb, *.accdb)}; Dbq=" + file;
                OdbcConnection odbcConnection = new OdbcConnection(connetionString);
                try
                {
                    odbcConnection.Open();
                    var schema = odbcConnection.GetSchema("Tables");

                    bool bolCheckTableExist = false;
                    foreach (System.Data.DataRow row in schema.Rows)
                    {
                        var tableName = row["TABLE_NAME"].ToString();
                        if (tableName.Equals(strTableName))
                        {
                            bolCheckTableExist = true;
                            break;
                        }
                    }

                    if (bolCheckTableExist)
                    {
                        string sqlQuery = "SELECT * FROM " + strTableName.Trim();
                        OdbcCommand objCommand = new OdbcCommand(sqlQuery, odbcConnection);
                        OdbcDataAdapter da = new OdbcDataAdapter(objCommand);

                        da.Fill(dtRsl);
                        if (dtRsl.Rows.Count > 0)
                        {
                            dtRsl.Columns.Add("IsExisted", typeof(int));//đánh dấu record đã tồn tại trong DB hay chưa
                            dtRsl.Columns.Add("IsDefault", typeof(int));//đánh dấu có giá CIF miền Nam
                            dtRsl.Columns.Add("IsDefaultNorth", typeof(int));//đánh dấu có giá CIF miền Bắc
                            dtRsl.Columns.Add("IsUpdateCurrentCIFSouth", typeof(int));//đánh dấu khi cần update lại các giá CIF miền Nam hiện tại về 0
                            dtRsl.Columns.Add("IsUpdateCurrentCIFNorth", typeof(int));//đánh dấu khi cần update lại các giá CIF miền Bắc hiện tại về 0
                        }
                        odbcConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                messageText = "Không tìm thấy tập tin!";
            }

            return dtRsl;
        }
    }
}
