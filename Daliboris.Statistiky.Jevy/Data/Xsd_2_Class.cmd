SET Cesta=V:\Projekty\Daliboris\Jevy\
SET XSD=jevy.xsd
SET NS=Daliboris.Statistiky

"C:\Program Files\Microsoft SDKs\Windows\v6.0A\Bin\Xsd.exe" %Cesta%Data\%XSD% /c /n:%NS% /o:%Cesta%

Pause