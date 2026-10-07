// rust/ c# communication

use std::io::(BufRead, BufReader, Write)
use std::process(Child, ChildStdin, ChildStdout, Command, Stdio )

pub struct Backend //hold stuff to communicate with other stuff (cs)
{
    process: Child
    stdin: ChildStdin,
    stdout: BufReader<ChildStdout>,
}

impl Backend //constructor for backend (think static void Main(...){})
{
    pub fn new() -> Result< Self, Box< dyn std::error::Error > >
    {
        let mut process = Command::new("dotnet")
            .arg("../Program.dll")
            .stdin(Studio::piped()) //create pipe
            .stdout(Studio::piped()) //create pipe
            .spawn()?;

        //take comm out and stores them
        let stdin = process.stdin.take().unwrap(); 
        let stdout = process,stdout.take().unwrap();
        
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
        meassage: &str,

    ) -> Result<String, Box<dyn std::error::Error>>

    riteln!(self.stdin, "{}", message)?;
    self.stdin.flush()?;
}