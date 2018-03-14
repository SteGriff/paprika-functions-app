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
 
## Auto-tweeting

 * User authorises Paprika to access their Twitter account
 * User sets up a tweet schedule ("Every 1 hour(s), post (query:) '[tweet]') initally the lower bound is 1 hour. They turn the Schedule On/Off switch to On.
 * Schedule is saved to the user's table row as `ScheduleMinuteInterval: 60`, `ScheduleQuery: [tweet]`, `ScheduleEnable: true`, `ScheduleLastPosted: 2017-01-01T00:00:00` (the last value is an arbitrary epoch which is guaranteed to be in the past)
 * Every 20 mins (initially) the new `ScheduledTweets` timed Function runs.
     - This is a slow job because it has to full-text-search every user so we won't make it do the hard work of actually tweeting
     - It finds every user where `ScheduleLastPosted + Minutes(ScheduleMinuteInterval) < DateTime.Now` and adds their `Username`, `ScheduleQuery`, and OAuth fields to a new Queue Message (in a new `TweetQueue` queue).
 * Every time a message is detected in `TweetQueue`, the new `PostTweets` Function runs 
     - It pulls out the details loaded above and uses the OAuth deets to authorise with Twitter.
	 - It resolves their `ScheduleQuery` into a result
	 - It posts the result to their twitter account
 
### New components:

 * New fields in User
 * New queue, "TweetQueue"
 * New Function, "ScheduleTweets"
 * New Function, "PostTweets"
 * New UI for setting up schedule