//starts ratatui and the c# backend

mod app;
mod backendcomms;
mod input;

use crossterm::event;

pub fn main() -> std::io::Result<()> {
    let mut app = app::App::new();

    loop {
        let event = event::read()?;

        if let Some(action) = input::action_map(event) {
            match action {
                input::Action::ToggleTimer => {
                    app.toggle_timer();

                    if app.timer_running() {
                        println!("timer started")
                    } else {
                        println!("timer stopped");
                        println!("Elasped: {:?}", app.elapsed());
                    }
                }
                input::Action::SwitchTab => {
                    println!("Tab switched");
                }
                input::Action::Quit => {
                    println!("app quit");
                    break;
                }
            }
        }
    }
    Ok(())
}
