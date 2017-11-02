# Paprika Functions App API Design

All requests must be authenticated with `username` and `password` in headers.

Users get one grammar file each. Pre-compilers can be used at the client side to put multiple files together. Perhaps the console app can gain a feature to compile and upload.

Grammar file is stored in a blob using the user's username, i.e. <username>.txt.
Blob therefore comprises flat list of the grammar files of all users.

## POST `/Grammar/Edit` (body)

Uploads a grammar (request body), replacing <username>.txt in the blob storage. The Function App authenticates to the blob using an SA Key.

## GET `/Grammar/Download`

Downloads the grammar of the currently authenticated user.

## GET `/Grammar/Resolve?q=`

Resolves the grammar passed as `q`

## POST `/Grammar/Resolve` (q, [values])

Resolves the grammar passed as `q` using the injected values. Each injected value takes its own POST parameter and is the value from within a Paprika `[bracketed expression]` *without* the brackets. So for example:

	HTTP/1.1 POST
	https://paprika.endpoint/Grammar/Resolve
	q=my favourite [category] is [[category]] [chatter]
	category=country
	chatter=for sure
	
You can specify as many or as few injected values as you want.