// Create IEnumerable<bool> to store individual LED data
List<bool> lstRow1 = [];
List<bool> lstRow2 = [];
List<bool> lstRow3 = [];
List<bool> lstRow4 = [];
List<bool> lstRow5 = [];
List<bool> lstRow6 = [];
List<bool> lstRow7 = [];

// Fill above data here

// Create IEnumerable<bool> to store individual LED data for the whole screen
List<bool> lstScreen =
[
	.. lstRow1,
	.. lstRow2,
	.. lstRow3,
	.. lstRow4,
	.. lstRow5,
	.. lstRow6,
	.. lstRow7,
];

ScreenData sd1 = new ScreenData(lstScreen);

// Using the per-row constructor is equivalent to the following:

ScreenData sd2 = new ScreenData();
sd2.FillFromBools(lstScreen);