using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Daliboris.Statistiky.Rozhrani.Jevy;

namespace Daliboris.Statistiky
{
    /// <summary>
    /// Jednotlivý jev
    /// </summary>
    /// 
    [DebuggerDisplay("{Jazyk}, {Nazev}, {Pocet}")]
    public class Jev : IJev, ICloneable
    {
        public Jev(string strJazyk, string strNazev)
        {
            Nazev = strNazev;
            Jazyk = strJazyk;
        }

        public Jev(string strJazyk, string strNazev, string strRetrograd)
            : this(strJazyk, strNazev)
        {
            Retrograd = strRetrograd;
        }

        public Jev(string strJazyk, string strNazev, string strRetrograd, int intPocet)
            : this(strJazyk, strNazev, strRetrograd)
        {
            Pocet = intPocet;
        }

        public Jev(string strJazyk, string strNazev, int intPocet)
            : this(strJazyk, strNazev)
        {
            Pocet = intPocet;
        }

        public Jev(string strJazyk, string strNazev, object objObsah)
            : this(strJazyk, strNazev)
        {
            Obsah = objObsah;
        }

        public Jev(string strJazyk, string strNazev, object objObsah, int intPocet)
            : this(strJazyk, strNazev, objObsah)
        {
            Pocet = intPocet;
        }

        public Jev(string strJazyk, string strNazev, object objObsah, string strRetrograd)
            : this(strJazyk, strNazev, objObsah)
        {
            Retrograd = strRetrograd;
        }

        public Jev(string strJazyk, string strNazev, object objObsah, string strRetrograd, int intPocet)
            : this(strJazyk, strNazev, objObsah, strRetrograd)
        {
            Pocet = intPocet;
        }


        public string Jazyk { get; set; }

        public string Nazev { get; set; }

        public string Retrograd { get; set; }

        public int Pocet { get; set; } = 1;

        public List<string> Kontexty { get; set; } = new List<string>();

        public object Obsah { get; set; }
        
        
        public override bool Equals(object obj) {
            Jev jv = obj as Jev;
            if (jv == null)
                return false;
            return String.Equals(Nazev, jv.Nazev);
        }

        public override int GetHashCode() {
            //return base.GetHashCode();
            int iHashCode = 0;
            if (Nazev != null)
                iHashCode = Nazev.GetHashCode();
            return iHashCode;
        }
        public static bool operator ==(Jev x, Jev y) {
            if ((object)x == null)
                return ((object)y == null);
            return x.Equals(y);
        }
        public static bool operator !=(Jev x, Jev y) {
            return !(x == y);
        }
        
        object ICloneable.Clone() {
            return this.Clone();
        }
        
        public virtual Jev Clone() {
            Jev jv = this.MemberwiseClone() as Jev;
            if (Obsah != null) {
                using (Stream objectStream = new MemoryStream()) {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(objectStream, Obsah);
                    objectStream.Seek(0, SeekOrigin.Begin);
                    jv.Obsah = (object)formatter.Deserialize(objectStream);
                }
            }
            return jv;

        }

    }
}