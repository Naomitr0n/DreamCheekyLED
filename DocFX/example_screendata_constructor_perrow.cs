// Create IEnumerable<bool> to store individual LED data
List<bool> lstRow1 = [];
List<bool> lstRow2 = [];
List<bool> lstRow3 = [];
List<bool> lstRow4 = [];
List<bool> lstRow5 = [];
List<bool> lstRow6 = [];
List<bool> lstRow7 = [];

// Fill above data here

ScreenData sd1 = new ScreenData(lstRow1, lstRow2, lstRow3, lstRow4, lstRow5, lstRow6, lstRow7);

// Using the per-row constructor is equivalent to the following:

ScreenData sd2 = new ScreenData();
sd2.FillFromBools(lstRow1, lstRow2, lstRow3, lstRow4, lstRow5, lstRow6, lstRow7);

// And also the following:

ScreenData sd3 = new ScreenData();
sd3.FillFromBools(RowNum: 1, lstRow1);
sd3.FillFromBools(2, lstRow2);
sd3.FillFromBools(3, lstRow3);
sd3.FillFromBools(4, lstRow4);
sd3.FillFromBools(5, lstRow5);
sd3.FillFromBools(6, lstRow6);
sd3.FillFromBools(7, lstRow7);