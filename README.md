# Revit Room Data Extractor (C#)

A professional Revit API plugin developed in C# that automatically extracts critical spatial data (Rooms) from any Revit model and exports it to a structured CSV file for further data analysis, PowerBI integration, or facility management.

## Features
- Bypasses unplaced or unbound rooms automatically.
- Extracts Room Number, Name, Area (converted to Metric/Imperial), and Volume.
- Fast and optimized using `FilteredElementCollector`.

## Tech Stack
- C# (.NET Framework)
- Revit API (`Autodesk.Revit.DB`, `Autodesk.Revit.UI`)
