// rust/ c# communication

use std::io::{BufRead, BufReader, Write};
use std::process::{Child, ChildStdin, ChildStdout, Command, Stdio };

pub struct Backend //hold stuff to communicate with other stuff (cs)
{
    process: Child,
    stdin: ChildStdin,
    stdout: BufReader<ChildStdout>,
}

impl Backend //constructor for backend (think static void Main(...){})
{
    pub fn new() -> Result< Self, Box< dyn std::error::Error > >
    {
        let mut process = Command::new("dotnet")
            .arg("../Program.dll")
            .stdin(Stdio::piped()) //create pipe
            .stdout(Stdio::piped()) //create pipe
            .spawn()?;

        //take comm out and stores them
        let stdin = process.stdin.take().unwrap(); 
        let stdout = process.stdout.take().unwrap();
        
        Ok //put everything onto backnde
        (
            Self
            {
                process,
                stdin, 
                stdout: BufReader::new(stdout)
            }
        )

    }

    pub fn send 
    (
        &mut self,
        message: &str,

    ) -> Result<String, Box<dyn std::error::Error>>
        {
            writeln!(self.stdin, "{}", message)?;
            self.stdin.flush()?;

            let mut response = String::new();
            let read = self.stdout.read_line(&mut response)?;

            if read == 0 
            {
                return Err(io::Error::new(io::ErrorKind::UnexpectedEof, "cs backend closed stdout").into());
            }

            Ok(response.trim_end().to_string())
        }

}