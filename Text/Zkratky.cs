using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Daliboris.Text {
	public static class Zkratky {
		const char cchTecka = '.';

		public static List<string> Identifikuj(string strText) {
			return Identifikuj(strText, false);

		}

		public static List<string> Identifikuj(string strText, bool blnVcetneKonceVety) {
			List<string> glsZkratky = new List<string>();
			int iTecka = strText.IndexOf(cchTecka);
			while (iTecka > 0) {
				//pokud je tečka poslední v textu, nejspíš nepůjde o zkratku
				if (iTecka + 2 >= strText.Length)
					break;

				//je následující znak mezera?
				if (Char.IsWhiteSpace(strText[iTecka + 1])) {
					//začíná znak za mezerou malým písmenem?
					if (!blnVcetneKonceVety ? Char.IsLower(strText, iTecka + 2) : true) {
						int iPozice = iTecka - 1;
						//while (iPozice >= 0 && Char.IsLower(strText, iPozice)) {
						//while (iPozice >= 0 && !Char.IsWhiteSpace(strText, iPozice)) {
						while (iPozice >= 0 && (Char.IsLetter(strText, iPozice) || strText[iPozice] == cchTecka) ) {
							iPozice--;
						}
						if(iTecka - iPozice > 1)
							glsZkratky.Add(strText.Substring(iPozice + 1, iTecka - iPozice));
					}
				}
				iTecka = strText.IndexOf(cchTecka, iTecka + 1);
			}
			return glsZkratky;

		}

	}
}
