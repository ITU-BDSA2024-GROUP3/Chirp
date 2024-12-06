# CSV database implementation of cheep
This is a implementation of Chirp, which uses a csv database and a terminal interface.

## Starting server
If the azure server is up and running, one does not need to start a server.

If the azure server is not up and running, one can start a local server by going to `Chirp/src/Chirp.CSVDBService/` in a terminal, and using the following command:

`dotnet run`

Then one would need to open the code in `Chirp/src/Chirp.CLI/Program.cs`, then outcomment line 52 and 76 ( `var baseURL = "https://bdsagroup3chirpremotedb.azurewebsites.net/"; `).
Then one should remove the comment markers (`//`) on line 53 and 77, if one starts the server on another port than described, put in that port number instead.

Now the local server should be accessible for posting and reading.

## Posting
To post a cheep to the database, one must open a terminal in `Chirp/src/Chirp.CLI/`, the folder of the client.
In here one can type the following command to post a cheep with the text `Hello World`:

`dotnet run -- cheep "Hello World"`

## Reading
To read the cheeps in the database, one must first open a terminal in `Chirp/src/Chirp.CLI/`, the folder of the client.
In here one can type one of two different commands to read cheeps. The first command will read all cheeps in the database,
the second will read a specified number of the newest cheeps.

`dotnet run -- read`

`dotnet run -- read 2`

## Bugs
It should be noted, that currently, if one wishes to run the program on a azure webservice, there will be a problem. 
To get it to work, one would need to start a new server from `Chirp/src/Chirp.CSVDBService`, then once the server is 
up, one needs to use the same command again, which will "update" the server, even if no changes have been made. This 
is a weird bug which does not appear on local host, so we have assumed that it's a problem with the way Azure sets up 
the server. 