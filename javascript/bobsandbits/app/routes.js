    var React = require('react');
    var Router = require('react-router');
    var DefaultRoute = Router.DefaultRoute;
    var Route = Router.Route;
    var routes = (
        <Route name="app" path="/" >
            <DefaultRoute handler={require('./components/LengthModule.jsx')} />
        </Route>
    ); 
    module.exports = routes;