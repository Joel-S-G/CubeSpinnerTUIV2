//app state
use std::time::{Duration, Instant};

pub struct App {
    timer_running: bool,
    timer_started: Option<Instant>,
    save_elapsed: Duration,
}

impl App {
    pub fn new() -> App {
        App {
            timer_running: false,
            timer_started: None,
            save_elapsed: Duration::ZERO,
        }
    }

    pub fn toggle_timer(&mut self) {
        if self.timer_running {
            if let Some(started) = self.timer_started {
                self.save_elapsed += started.elapsed();
            }

            self.timer_running = false;
            self.timer_started = None;
        } else {
            self.timer_running = true;
            self.timer_started = Some(Instant::now());
        }
    }

    pub fn timer_running(&self) -> bool {
        self.timer_running
    }

    pub fn elapsed(&self) -> Duration {
        match self.timer_started {
            Some(started) => self.save_elapsed + started.elapsed(),
            None => self.save_elapsed,
        }
    }
}
