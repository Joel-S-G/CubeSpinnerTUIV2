// input handling

use crossterm::event::{Event, KeyCode, KeyEventKind};

pub enum Action {
    ToggleTimer,
    SwitchTab,
    Quit,
}

//maps actions to an input
pub fn action_map(event: Event) -> Option<Action> {
    match event {
        Event::Key(key_event) => {
            if key_event.kind != KeyEventKind::Press {
                return None;
            }
            match key_event.code {
                KeyCode::Char(' ') => Some(Action::ToggleTimer),
                KeyCode::Tab => Some(Action::SwitchTab),
                KeyCode::Char('q') => Some(Action::Quit),
                _ => None,
            }
        }
        _ => None,
    }
}
