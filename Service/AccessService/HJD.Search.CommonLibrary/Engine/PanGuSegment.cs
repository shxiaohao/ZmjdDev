using HJD.Search.CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using PanGu;
using System.Data;

namespace HJD.Search.CommonLibrary.Engine
{
    public class PanGuSegment
    {
        public PanGuSegment()
        {
            //DB.UpdatePGDictWithWHotelWord();

            DataTable dt = GetAllGPDict();
            List<WordAttribute> waList = new List<WordAttribute>();
            foreach (DataRow row in dt.Rows)
            {
                WordAttribute wa = new WordAttribute()
                {
                    Word = row[0].ToString(),
                    Pos = GenPos(row[1].ToString()),
                    Frequency = (int)row[2]
                };
                waList.Add(wa);
            }

            PanGu.Segment.InitWithDictList(waList);
        }

        public POS GenPos(string posList)
        {
            int ipos = 0;
            if (posList.IndexOf(",") > 0)
            {
                foreach (string pos in posList.Split(','))
                {
                    ipos += GetPosInt(pos);
                }
            }
            else
            {
                ipos = GetPosInt(posList);
            }

            return (POS)ipos;
        }

        public int GetPosInt(string pos)
        {
            switch (pos.ToUpper())
            {
                case "UNK": return 0; break;
                case "K": return 2; break;
                case "H": return 4; break;
                case "NZ": return 8; break;
                case "NX": return 16; break;
                case "NT": return 32; break;
                case "NS": return 64; break;
                case "NR": return 128; break;
                case "Z": return 256; break;
                case "Y": return 512; break;
                case "X": return 1024; break;
                case "W": return 2048; break;
                case "V": return 4096; break;
                case "U": return 8192; break;
                case "T": return 16384; break;
                case "S": return 32768; break;
                case "R": return 65536; break;
                case "Q": return 131072; break;
                case "P": return 262144; break;
                case "O": return 524288; break;
                case "N": return 1048576; break;
                case "MQ": return 2097152; break;
                case "M": return 4194304; break;
                case "L": return 8388608; break;
                case "I": return 16777216; break;
                case "F": return 33554432; break;
                case "E": return 67108864; break;
                case "D": return 134217728; break;
                case "C": return 268435456; break;
                case "B": return 536870912; break;
                case "A": return 1073741824; break;
                default:
                    return 0;
                    break;
            }
        }

        PanGu.Segment segment = new PanGu.Segment();

        public ICollection<WordInfo> GetSegment(string comment)
        {

            return segment.DoSegment(comment);
        }

        public static DataTable GetAllGPDict()
        {
            string sql = "SELECT * FROM PGDict p ";

            return SqlHelper.Select(SqlHelper.CommentDbConn, sql).Tables[0];
        }
    }
}
