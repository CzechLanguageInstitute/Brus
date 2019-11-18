# Brus

Nástroj pro analýzu obsahu wordovských dokumentů. Rozloží text na odstavce, úseky označené znakovým stylem, dále na slova a jednotlivé znaky a jejich kombinace.

## Getting Started
### Prerequisities

- Microsoft Windows
- Microsoft Visual Studio 2017
  - .NET Framework 4.0 Client Profile
- Git (version 2.19 or newer)
- JetBrains ReSharper (recommended)
- [Microsoft Ribbon for WPF October 2010](https://www.microsoft.com/en-us/download/details.aspx?id=11877) 

### Installing

## Building

TODO

## Deployment

TODO

## Versioning

TODO

## Testing

Složka **Tests** obsahuje testovací data: wordovský dokument **AdamM.docx** a dokument **AdamM.pjv**, který obsahuje statistická data o dokumentu.

-  Spusťte program **Brus**.
-  Klikněte na tlačítko **Otevřít** a vyberte soubor **Adam.pjv**

Po načtení dokumentu se v levé části objeví název souboru. Po rozkliknutí se zobrazí dvě hlavní části: **Odstavcové styly** a **Znakové styly** spolu s údaji o počtu různých částí textu v dané oblasti.

Kliknutím na řádek s názvem odstavcového nebo znakového stylu se v pravé části zobrazí seznam znaků, slov, úseků a kombinace písmen.

Kliknutím na některou položku v seznamu **Znaky**, **Slova**, **Úseky**, **Digramy**, **Trigramy** se vyfiltrují zbývající části podle vybrané hodnoty.


## Authors

* **Boris Lehečka** - *The first version of Brus and architecture* 
