[![Circle CI](https://circleci.com/gh/rangle/angular2-redux-example.svg?style=svg)](https://circleci.com/gh/rangle/angular2-redux-starter)

# COE Registration 17.1 Client Web Application

## npm scripts

> To see all available scripts:
```bash
$ npm run
```

### Dev
```bash
$ npm run dev
```
This runs a development mode server with live reload etc. Linter warnings will
be displayed with each reload.

Open `http://localhost:8080` in your browser.

### Production

```bash
$ npm install
$ npm start
```

This runs a production-ready express server that serves up a bundled and
minified version of the client.

Open `http://localhost:8080` in your browser.

### Tests

#### Single Run (with linting and coverage)
```bash
$ npm test
# or
$ npm t
```

#### Watch Files
```bash
$ npm run test:watch
```

#### Linting
```bash
$ npm run lint
```
This will run both code and style linters, but you can run them individually 
using `npm run lint-ts` and `npm run lint-css`.

#### Coverage
```bash
$ npm run cover
```

#### Connecting to remote APIs

Both the devmode and production servers provide a way to proxy requests to
remote HTTP APIs.  This can be useful for working around CORS issues when
developing your software.

Edit [this file](server/proxy-config.js) to mount such APIs at a given path.

## License

Copyright &copy;2019 PerkinElmer, Inc.
