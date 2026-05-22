# 🎱 TotoAnalyzer
### Bulgarian Sport Toto — Historical Draw Statistics & Analysis
TotoAnalyzer is a C# console application for analyzing historical draw data from the Bulgarian Sport Toto lottery. It fetches draw results across a user-defined time range and provides several statistical tools to uncover patterns in the numbers.
Features

Period selection — load draw data for any range of years since Sport Toto's inception
Top N most frequent numbers — find which numbers have been drawn most often, displayed as a bar chart
Hot pairs — discover which number combinations appear together most frequently
Decade distribution — see how draws are spread across number ranges (1–10, 11–20, etc.)
Heat map — a visual frequency map of all numbers across the selected period

Tech Stack

Language: C# (.NET)
Architecture: modular design with separate classes for data loading (DataLoader), statistics (Statistics), and visualization (Visualizer)
Interface: interactive CLI menu with Bulgarian-language prompts

Getting Started

1. Clone the repository
2. Build and run with dotnet run
3. Select option 1 to load a time period, then explore the statistics via the menu
