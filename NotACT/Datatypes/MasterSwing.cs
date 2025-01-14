﻿using System.Diagnostics;

namespace Advanced_Combat_Tracker {
    public class MasterSwing : IComparable, IComparable<MasterSwing> {
        public delegate string StringDataCallback(MasterSwing Data);

        public delegate Color ColorDataCallback(MasterSwing Data);

        public class ColumnDef {
            public StringDataCallback GetCellData;

            public StringDataCallback GetSqlData;

            public Comparison<MasterSwing> SortComparer;

            public ColorDataCallback GetCellForeColor = (MasterSwing Data) => Color.Transparent;

            public ColorDataCallback GetCellBackColor = (MasterSwing Data) => Color.Transparent;

            public string SqlDataType { get; }

            public string SqlDataName { get; }

            public bool DefaultVisible { get; }

            public string Label { get; }

            public ColumnDef(string Label, bool DefaultVisible, string SqlDataType, string SqlDataName, StringDataCallback CellDataCallback, StringDataCallback SqlDataCallback, Comparison<MasterSwing> SortComparer) {
                this.Label = Label;
                this.DefaultVisible = DefaultVisible;
                this.SqlDataType = SqlDataType;
                this.SqlDataName = SqlDataName;
                GetCellData = CellDataCallback;
                GetSqlData = SqlDataCallback;
                this.SortComparer = SortComparer;
            }
        }

        public class DualComparison : IComparer<MasterSwing> {
            private string sort1;

            private string sort2;

            public DualComparison(string Sort1, string Sort2) {
                sort1 = Sort1;
                sort2 = Sort2;
            }

            public int Compare(MasterSwing? Left, MasterSwing? Right) {
                var num = 0;
                Debug.Assert(Left != null, nameof(Left) + " != null");
                Debug.Assert(Right != null, nameof(Right) + " != null");
                if (ColumnDefs.ContainsKey(sort1)) {
                    num = ColumnDefs[sort1].SortComparer(Left, Right);
                }
                if (num == 0 && ColumnDefs.ContainsKey(sort2)) {
                    num = ColumnDefs[sort2].SortComparer(Left, Right);
                }
                if (num == 0) {
                    num = ((Left.TimeSorter == Right.TimeSorter) ? Left.Time.CompareTo(Right.Time) : Left.TimeSorter.CompareTo(Right.TimeSorter));
                }
                return num;
            }
        }

        public static Dictionary<string, ColumnDef> ColumnDefs = new Dictionary<string, ColumnDef>();

        internal DateTime time;

        internal Dnum damage;

        internal string attackType;

        internal string attacker;

        internal string damageType;

        internal string victim;

        internal bool critical;

        internal int timeSorter;

        internal int swingType;

        internal string special;

        public string Special => special;

        public EncounterData ParentEncounter { get; set; }

        public DateTime Time => time;

        public int TimeSorter => timeSorter;

        public int SwingType => swingType;

        public Dnum Damage => damage;

        public string Attacker => attacker;

        public string Victim => victim;

        public string AttackType => attackType;

        public string DamageType => damageType;

        public bool Critical {
            get => critical;
            set => critical = value;
        }

        public static string[] ColTypeCollection {
            get {
                var array = new string[ColumnDefs.Count];
                var num = 0;
                foreach (var columnDef in ColumnDefs) {
                    array[num] = columnDef.Value.SqlDataType;
                    num++;
                }
                return array;
            }
        }

        public static string[] ColHeaderCollection {
            get {
                var array = new string[ColumnDefs.Count];
                var num = 0;
                foreach (var columnDef in ColumnDefs) {
                    array[num] = columnDef.Value.SqlDataName;
                    num++;
                }
                return array;
            }
        }

        public static string ColHeaderString => string.Join(",", ColHeaderCollection);

        public string[] ColCollection {
            get {
                var array = new string[ColumnDefs.Count];
                var num = 0;
                foreach (var columnDef in ColumnDefs) {
                    array[num] = columnDef.Value.GetSqlData(this);
                    num++;
                }
                return array;
            }
        }

        public Dictionary<string, object> Tags { get; set; } = new Dictionary<string, object>();

        public MasterSwing(int SwingType, bool Critical, Dnum damage, DateTime Time, int TimeSorter, string theAttackType, string Attacker, string theDamageType, string Victim) {
            time = Time;
            this.damage = damage;
            attacker = Attacker;
            victim = Victim;
            attackType = theAttackType;
            damageType = theDamageType;
            critical = Critical;
            timeSorter = TimeSorter;
            swingType = SwingType;
            special = "specialAttackTerm-none";
        }

        public MasterSwing(int SwingType, bool Critical, string Special, Dnum damage, DateTime Time, int TimeSorter, string theAttackType, string Attacker, string theDamageType, string Victim) {
            time = Time;
            this.damage = damage;
            attacker = Attacker;
            victim = Victim;
            attackType = theAttackType;
            damageType = theDamageType;
            critical = Critical;
            timeSorter = TimeSorter;
            swingType = SwingType;
            special = Special;
        }

        public string GetColumnByName(string name) => ColumnDefs.ContainsKey(name) ? ColumnDefs[name].GetCellData(this) : string.Empty;

        public override string ToString() {
            return $"{Time:s}|{Damage}|{Attacker}|{Special}|{AttackType}|{DamageType}|{Victim}";
        }

        public override bool Equals(object? obj) {
            var masterSwing = (MasterSwing)obj!;
            var text = ToString();
            var value = masterSwing.ToString();
            return text.Equals(value);
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public int CompareTo(object? obj) {
            return CompareTo(((MasterSwing?)obj));
        }

        public int CompareTo(MasterSwing? other) {
            var num = 0;
            Debug.Assert(other != null, nameof(other) + " != null");
            if (ColumnDefs.ContainsKey(ActGlobals.aTSort)) {
                num = ColumnDefs[ActGlobals.aTSort].SortComparer(this, other);
            }
            if (num == 0 && ColumnDefs.ContainsKey(ActGlobals.aTSort2)) {
                num = ColumnDefs[ActGlobals.aTSort2].SortComparer(this, other);
            }
            if (num == 0) {
                num = ((TimeSorter == other.TimeSorter) ? Time.CompareTo(other.Time) : TimeSorter.CompareTo(other.TimeSorter));
            }
            return num;
        }

        internal static int CompareTime(MasterSwing Left, MasterSwing Right) {
            var num = Left.TimeSorter.CompareTo(Right.TimeSorter);
            if (num == 0) {
                num = Left.Time.CompareTo(Right.Time);
            }
            return num;
        }
    }
}
