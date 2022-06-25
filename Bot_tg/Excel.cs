using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace Bot_tg
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class Excel
    {
        static Regex remove = new Regex(@"\-[0-9]*");
        string path = "";
        _Application excel = new Application();
        Workbook wb;
        Worksheet ws;
        _Excel.Range rng;
        public Excel(string path, int Sheet)
        {
            this.path = path;
            wb = excel.Workbooks.Open(path);
            ws = wb.Worksheets[Sheet];
        }
        public string ReadCell(int i, int j)
        {
            if (ws.Cells[i, j].Value != null)
                return ws.Cells[i, j].Value.ToString();
            else return "";
        }
        public string Is(int i, int j)
        {
            rng = ws.Cells[i, j];
            if (rng.MergeCells)
                return rng.MergeArea.Address.ToString();
            else return "";
        }
        public void Close()
        {
            wb.Close();
            wb = null;
            ws = null;
            excel.Quit();
            excel = null;
            GC.Collect();
        }
        public List<string> CreateGroups(string start = "")
        {
            List<string> list = new List<string>();
            int i = 3;
            if (start.Length > 0) start += "-";
            while (ReadCell(3, i) != "")
            {
                if (ReadCell(3, i).StartsWith(start))
                    list.Add(ReadCell(3, i));
                i++;
            }
            return list;
        }
        public List<string> CreatePrograms()
        {
            List<string> list = CreateGroups();
            List<string> l = new List<string>();
            string temp = "";
            foreach (string item in list)
            {
                string str = item;
                str = remove.Replace(str, "");
                if (temp != str)
                {
                    temp = str;
                    l.Add(temp);
                }
            }
            return l;
        }
        public Dictionary<string, List<Para>> CreateTable()
        {
            Dictionary<string, List<Para>> TimeTable = new Dictionary<string, List<Para>>();
            int i = 3;
            int j = 3;
            string group = ReadCell(3, j);
            while (group != "")
            {
                i = 4;
                TimeTable.Add(group, new List<Para>());
                while (ReadCell(i, 2) != "")
                {
                    if (ReadCell(i, j) != "")
                    {
                        var day = ReadCell(i, 1);
                        var time = ReadCell(i, 2);
                        var description = ReadCell(i, j);
                        var _new = new Para(day, time, description);
                        TimeTable[group].Add(_new);
                    }
                    i++;
                }
                j++;
                group = ReadCell(3, j);
            }
            return TimeTable;
        }
        public static List<string> CompareTimeTable(Dictionary<string, List<Para>> NewTimeTable, Dictionary<string, List<Para>> OldTimeTable)
        {
            List<string> defferense = new List<string>();
            foreach (var item in NewTimeTable.Keys)
            {
                if (OldTimeTable.Keys.Contains(item))
                {
                    if (OldTimeTable[item].All(x => NewTimeTable[item].Contains(x)) && NewTimeTable[item].All(x => OldTimeTable[item].Contains(x)))
                    {
                    }
                    else
                    {
                        defferense.Add(item);
                    }
                }
                else
                {
                    defferense.Add(item);
                }
            }
            return defferense;
        }
    }
}
