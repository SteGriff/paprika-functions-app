# Anaonymous usage flow

## Use cases

**As an anonymous first-time user:**

When I arrive on Paprika, I want the grammar to show a sample grammar, and I want to be encouraged to enter some test queries which introduce me to the language;

When I make changes to the sample grammar, I want them to be saved and persisted;

I want to be able to keep my changes and upgrade to a real account with a name that I choose.

## User interface implementation

When the page loads, if the username and password fields are not filled in, then a server call for an anonymous user will be sent and the sample grammar and tutorial will be populated.

Otherwise, we will attempt to load the content and settings for the currently filled-in credentials.