# MINI KeyBoard – bewerkbare broncode

De map `MINI-KeyBoard-source` bevat de teruggehaalde en bouwbare C#-broncode van de hoofdapplicatie.

## Openen en bouwen

Open `MINI-KeyBoard-source/MINI KeyBoard.csproj` in Visual Studio 2022 of hoger en kies **Build**. De gebouwde app staat vervolgens in `MINI-KeyBoard-source/bin/Debug/net461/`.

De hoofdapp is gemigreerd van .NET Framework 4.0 naar 4.6.1, zodat zij op deze computer reproduceerbaar kan bouwen. De HID-bibliotheken zijn als lokale afhankelijkheden opgenomen in `dependencies`; de oorspronkelijke downloadmap is niet nodig.

## Waar je features toevoegt

- `MINI-KeyBoard-source/HIDTester/FormMain.cs`: hoofdvenster, menu, toetsen, HID-berichten en opslaan naar het apparaat.
- `MINI-KeyBoard-source/HIDTester/BasicKeys.cs`, `FunKey.cs`, `MULKey.cs`, `MouseKey.cs`, `LayerFun.cs`: afzonderlijke instelvensters.
- `MINI-KeyBoard-source/HIDTester/LedStudioControl.cs`: het nieuwe LED Studio-scherm met presets, kleur, helderheid, snelheid en firmwaremoduswaarde.
- `MINI-KeyBoard-source/HIDTester/HidLib.cs`: verbinding en schrijven via HID.

`HidLibrary-source` en `Theraot.Core-source` zijn ook gedecompileerde referentiebroncode. De app bouwt doelbewust tegen de gecontroleerde DLL-kopieën in `dependencies`, zodat wijzigingen aan de hoofdapp niet worden gehinderd door eventuele decompileerafwijkingen in die externe bibliotheken.

## Controle

De hoofdapp is succesvol opgebouwd. Test nieuwe code eerst met het apparaat losgekoppeld of met een testprofiel; de knop voor downloaden schrijft instellingen naar het toetsenbord. Het bestaande protocol bevestigt alleen LED-moduswaarden `0`, `1` en `2`; andere waarden en de visuele kleur-/snelheidsinstellingen vereisen firmwareondersteuning voordat ze fysiek naar het toetsenbord kunnen worden verzonden.
