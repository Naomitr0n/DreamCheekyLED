// Create IEnumerable<bool> to store individual LED data
List<bool> lstRow1 = [];
List<bool> lstRow2 = [];
List<bool> lstRow3 = [];
List<bool> lstRow4 = [];
List<bool> lstRow5 = [];
List<bool> lstRow6 = [];
List<bool> lstRow7 = [];

// Fill above data here

ScreenData sd = new ScreenData();

sd.FillRowFromBools(RowNum: 1, false, false, ..., false, false); // provide exactly 21 bool values
sd.FillRowFromBools(2, lstRow2);
// Fill rows 3-7 using FillRowFromBools() overloads