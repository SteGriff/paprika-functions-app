# Service Architecture

## Future (planned) operation

In the perfect design for the upload system, we will take the following steps:

 * The grammar file is uploaded to `/Grammar/Edit` endpoint. 
 * File is written to durable blob storage with username and ISO 8601 timestamp
 * A message is written to a storage queue with the timestamped filename. As the grammar files will often be more than 64KB in size, we will write a compact message rather than the entire file to process
 * A separate Azure Function 'B' (`/Internal/Cache`?) will watch the storage queue for new messages
 * Function 'B' will look up the file from durable storage and `Core.Parse` it into a Paprika Grammar structure.
 * Function 'B' will write the Paprika Grammar struct to table storage ('grammar') with the username-timestamp (or just username?) as the PartitionKey and the time as the RowKey
 
When parsing a grammar:

 * The grammar parser will search the 'grammar' table storage for the *latest* grammar struct for the user
 * This grammar will be directly loaded into a new Core and used to return the response
 
## Current operation

 * The `/Grammar/Edit` endpoint will receive the grammar file, parse it to a PG Struct, and load that into table storage.
 * The parser will pull up the latest (only?) grammar from table storage, rehydrate it, and use it to parse the query.