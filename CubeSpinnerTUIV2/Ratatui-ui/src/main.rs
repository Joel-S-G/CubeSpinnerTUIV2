//starts ratatui and the c# backend

use color_eyre::Result;
use crossterm::event::{self, Event};
use ratatui::{DefaultTerminal, Frame};

fn main() -> Result <()>
{
    color_eyre::install()?;
    let terminal = ratatui::init();
    let result = run(terminal);
    ratatui::restore();
    result
}

fn render(frame: &mut Frame)
{
    frame.render_widget
    (
        ratatui::widgets::Paragraph::new("CubeSpinner"),
         frame.area(),
    )
}

fn run(mut terminal: DefaultTerminal) -> Result<()>
{
    loop
    {
        terminal.draw(render)?;
        if let Event::Key(key) = event::read()?
        {
            match key.code
            {
               event::KeyCode::Char(c) =>
                {
                    if c == 'q'
                    {
                        break Ok(());
                    }
                }
                _=>{}
            }
        }
    }
}