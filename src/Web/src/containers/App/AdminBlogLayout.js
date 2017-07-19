// libs
import React, { Component } from "react";
import { Switch, Route } from "react-router-dom";

// our components
import Header from "../../components/Header";
import Footer from "../../components/Footer";

// our containers
import Restricted from "../../auth/Restricted";
import Setting from "../Setting/Setting";

export default class AdminBlogLayout extends Component {
  render() {
    const { match } = this.props;
    return (
      <div className="app">
        <Header />
        <div className="container">
          <div className="row">
            <div className="col-sm-12 blog-main">
              <Switch>
                <Route
                  exact
                  path={`${match.url}/setting`}
                  key="setting"
                  render={routeProps =>
                    <Restricted {...routeProps}>
                      {React.createElement(Setting)}
                    </Restricted>}
                />
              </Switch>
            </div>
          </div>
        </div>
        <Footer />
      </div>
    );
  }
}
