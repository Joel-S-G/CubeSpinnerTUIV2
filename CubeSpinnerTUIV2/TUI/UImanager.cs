//using as migration map

using Terminal.Gui;
using CubeSpinner;
using System.Data;

// app.rs / backend state
// application state owned by the C# backend or mirrored by Rust.
NxNxN_Scrambler scrambler = new();
CubeSpinner.Timer timer = new();
Session session = new();
StatisticsCalc statistics = new();

string currentScramble = scrambler.GenerateScramble();
SolveRecord? pendingSolve = null;

bool awaitingPenalty = false;
bool spaceIsDown = false;

// ui.rs / display formatting
// convert backend values into strings for the Ratatui screen.
string FormatStatistic(TimeSpan? value) => value?.ToString(@"m\:ss\.ff") ?? "--";
string FormatTimer(TimeSpan value) => $"{(int)value.TotalMinutes:00}.{value.Seconds:00}.{value.Milliseconds / 10:00}";

//  main.rs / terminal lifecycle
// Ratatui initialisation, terminal restoration, and main event loop here.
Application.Init();

ColorScheme terminalScheme = new()
{
	Normal = Application.Driver.MakeAttribute(Color.Gray, Color.Black),
	Focus = Application.Driver.MakeAttribute(Color.White, Color.Black),
	HotNormal = Application.Driver.MakeAttribute(Color.White, Color.Black),
	HotFocus = Application.Driver.MakeAttribute(Color.White, Color.Black),
	Disabled = Application.Driver.MakeAttribute(Color.DarkGray, Color.Black)
};
Application.Top.ColorScheme = terminalScheme;


//  ui.rs / timer screen layout
// Terminal.Gui controls become Ratatui widgets drawn every frame.
//layout-------------------------------------------------------------------------

Window window = new("Cube Spinner")
{
	X = 0,
	Y = 1,
	Width = Dim.Fill(),
	Height = Dim.Fill()
};

Label scrambleLabel = new($"Scramble: {currentScramble}")
{
	X = 1,
	Y = 1,
	Width = Dim.Fill() - 2
};

Label timerLabel = new("00.00.00")
{
	X = Pos.Center(),
	Y = 3,
	Width = 12,
	Height = 1
};

Label statusLabel = new("Press Space to start")
{
	X = Pos.Center(),
	Y = 5,
	Width = 30,
	Height = 1
};

TextView historyView = new()
{
	X = 1,
	Y = 7,
	Width = Dim.Percent(50) - 2,
	Height = Dim.Fill(3),
	ReadOnly = true,
	WordWrap = false,
	CanFocus = false,
	TabStop = false
};

TextView statisticsView = new()
{
	X = Pos.Right(historyView) + 2,
	Y = 7,
	Width = Dim.Fill(2),
	Height = Dim.Fill(3),
	ReadOnly = true,
	WordWrap = false,
	CanFocus = false,
	TabStop = false
};

Label helpLabel = new("Space: start/stop   R: new scramble   1: OK   2: +2   3: DNF   Q: quit")
{
	X = 1,
	Y = Pos.AnchorEnd(2),
	Width = Dim.Fill() - 2
};

//  ui.rs / solve-record table model and layout
// Ratatui does not retain TableView; build the table from current backend state during each draw.
//end of layout---------------------------------------------------------------------------------

DataTable solveTable = new(); //table of results
solveTable.Columns.Add("#", typeof(int));
solveTable.Columns.Add("Scramble", typeof(string));
solveTable.Columns.Add("Raw time", typeof(string));
solveTable.Columns.Add("Final time", typeof(string));
solveTable.Columns.Add("Solved at", typeof(string));
solveTable.Columns.Add("Penalty", typeof(string));

TableView solveTableView = new(solveTable)
{
	X = 1,
	Y = 1,
	Width = Dim.Fill(2),
	Height = Dim.Fill(2),
	CanFocus = false,
	TabStop = false
};

TabView tabs = new()
{
	X = 0,
	Y = 0,
	Width = Dim.Fill(),
	Height = Dim.Fill()
};

tabs.AddTab(new TabView.Tab("Timer", window), true);
tabs.AddTab(new TabView.Tab("Solve records", solveTableView), false);

//  backend state snapshot plus ui.rs rendering data
// The backend should return solves and statistics. Rust should format and draw in ui.rs.
void UpdateHistory() //rebuilds table per solve
{
	List<string> lines = new();
	for (int i = 0; i < session.solves.Count; i++)
	{
		SolveRecord solve = session.solves[i];
		string finalTime = solve.FinalTime is TimeSpan time ? FormatTimer(time) : "DNF";
		lines.Add($"{i + 1,3}: {finalTime} {solve.penaltyStatus}");
	}

	historyView.Text = lines.Count == 0 ? "No solves yet" : string.Join(Environment.NewLine, lines);
	statisticsView.Text = $"Mo3:  {FormatStatistic(statistics.Mo3)}\n" +
		$"Ao5:  {FormatStatistic(statistics.Ao5)}\n" +
		$"Ao12: {FormatStatistic(statistics.Ao12)}\n" +
		$"Ao100:{FormatStatistic(statistics.Ao100)}";

	solveTable.Rows.Clear();
	for (int i = 0; i < session.solves.Count; i++)
	{
		SolveRecord solve = session.solves[i];
		string finalTime = solve.FinalTime is TimeSpan time ? FormatTimer(time) : "DNF";
		solveTable.Rows.Add(
			i + 1,
			solve.Scramble ?? "",
			FormatTimer(solve.RawTime),
			finalTime,
			solve.SolvedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
			solve.penaltyStatus.ToString());
	}
	solveTableView.Update();
}

//  Backend/CubeSpinnerBackend.cs command handler
// domain behaviour, not UI drawing. Rust sends a NewScramble command.
void NewScramble() //new scramble generation
{
	currentScramble = scrambler.GenerateScramble();
	scrambleLabel.Text = $"Scramble: {currentScramble}";
	timerLabel.Text = "00.00.00";
	statusLabel.Text = "Press Space to start";
}

// Backend/CubeSpinnerBackend.cs command handler
// This starts the timer state machine. The Rust UIsends SpaceDown and displays the returned state.
async Task BeginSolve() // 
{
	timer.spaceDown();
	timerLabel.Text = "00.00.00";
	statusLabel.Text = "Get ready...";
	await Task.Delay(timer.startDelayMS + 50);

	Application.MainLoop.Invoke(() =>
	{
		if (timer.State == timerState.isHolding || timer.State == timerState.isReady)
		{
			statusLabel.Text = "Release Space to start";
		}
	});
}

// backend state updates
// Do not update Terminal.Gui controls from this event in new design.
// expose the latest timer state to Rust through the backend protocol.
timer.Statechanged += () =>
{
	if (timer.State == timerState.isRunning)
	{
		Application.MainLoop.Invoke(() => timerLabel.Text = FormatTimer(timer.ElapsedTime));
	}
};

// backend solve-completed handling
// Create the pending solve in the backend and return state showing that a penalty is required.
timer.SolveCompleted += elapsedTime =>
{
	Application.MainLoop.Invoke(() =>
	{
		timerLabel.Text = FormatTimer(elapsedTime);
		pendingSolve = new SolveRecord(elapsedTime, currentScramble);
		awaitingPenalty = true;
		statusLabel.Text = "Choose 1: OK, 2: +2, or 3: DNF";
	});
};

// input.rs plus backend command protocol
// The Rust input layer handles key events/sends commands instead of calling UI callbacks.
Application.Top.KeyDown += args =>
{
	Key key = args.KeyEvent.Key;
	args.Handled = true;

	if (key == Key.Q || key == Key.q)
	{
		Application.RequestStop();
		return;
	} //quits application, if you didn't figure this out you probably shouldn't be reading this source code

	if ((key == Key.R || key == Key.r) && !awaitingPenalty && 
		timer.State != timerState.isRunning &&
		timer.State != timerState.isHolding &&
		timer.State != timerState.isReady) //reloading the sramble
	{
		NewScramble();
		return;
	}

	//penalty assignment keybinds 
	if (awaitingPenalty && pendingSolve is not null)
	{
		if (key == Key.D1)
		{
			pendingSolve.penaltyStatus = SolveRecord.Penalty.OK;
		}
		else if (key == Key.D2)
		{
			pendingSolve.penaltyStatus = SolveRecord.Penalty.Plus2;
		}
		else if (key == Key.D3)
		{
			pendingSolve.penaltyStatus = SolveRecord.Penalty.DNF;
		}
		else
		{
			return;
		}

		session.AddSolve(pendingSolve);
		statistics.RecordSolve(pendingSolve);
		pendingSolve = null;
		awaitingPenalty = false;
		UpdateHistory();
		NewScramble();
		return;
	}

	if (key != Key.Space || spaceIsDown)
	{
		return;
	}

	spaceIsDown = true;

	if (timer.State == timerState.isIdle || timer.State == timerState.isStopped)
	{
		_ = BeginSolve();
	}
	else if (timer.State == timerState.isRunning)
	{
		timer.spaceDown();
	}
};

// input.rs plus backend command protocol
// Crossterm key-release handling maps to the backend's SpaceUp command.
Application.Top.KeyUp += args =>
{
	if (args.KeyEvent.Key != Key.Space)
	{
		return;
	}

	args.Handled = true;
	spaceIsDown = false;

	if (timer.State == timerState.isHolding || timer.State == timerState.isReady)
	{
		timer.spaceUp();
		statusLabel.Text = timer.State == timerState.isRunning
			? "Solving..."
			: "Press Space to start";
	}
};

//  main.rs / terminal shutdown
// Ratatui owns terminal cleanup. No Terminal.Gui controls are created by the backend.
window.Add(scrambleLabel, timerLabel, statusLabel, historyView, statisticsView, helpLabel);
window.ColorScheme = terminalScheme;
solveTableView.ColorScheme = terminalScheme;
tabs.ColorScheme = terminalScheme;
UpdateHistory();
Application.Top.Add(tabs);
Application.Run();
Application.Shutdown();