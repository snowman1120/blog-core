﻿// ref to https://github.com/IdentityModel/oidc-client-js/blob/dev/samples/VanillaJS/public/code-identityserver-sample.js
///////////////////////////////
// config
///////////////////////////////
Oidc.Log.logger = console;
Oidc.Log.level = Oidc.Log.DEBUG;

var url = window.location.origin;
console.log('root origin URL at ' + url);

var settings = {
    authority: 'http://localhost:5001',
    client_id: 'spa',
    redirect_uri: url + '/callback',
    post_logout_redirect_uri: url,
    response_type: 'id_token token',
    scope: 'openid profile api1',

    popup_redirect_uri: url + '/callback',
    popup_post_logout_redirect_uri: url,

    silent_redirect_uri: url + '/silent',
    automaticSilentRenew: false,

    filterProtocolClaims: true,
    loadUserInfo: true,
    revokeAccessTokenOnSignout: true
};
var mgr = new Oidc.UserManager(settings);

///////////////////////////////
// events
///////////////////////////////
mgr.events.addAccessTokenExpiring(function () {
    console.log("token expiring");

    // maybe do this code manually if automaticSilentRenew doesn't work for you
    mgr.signinSilent().then(function (user) {
        console.log("silent renew success", user);
    }).catch(function (e) {
        console.error("silent renew error", e.message);
    })
});

mgr.events.addAccessTokenExpired(function () {
    console.log("token expired");
});

mgr.events.addSilentRenewError(function (e) {
    console.log("silent renew error", e.message);
});

mgr.events.addUserLoaded(function (user) {
    console.log("user loaded", user);
    mgr.getUser().then(function () {
        console.log("getUser loaded user after userLoaded event fired");
    });
});

mgr.events.addUserUnloaded(function (e) {
    console.log("user unloaded");
});

///////////////////////////////
// functions for UI elements
///////////////////////////////

function getUserInfo() {
    return mgr.getUser().then(function (user) {
        console.log("getUserInfo success");
        if (user == null) return {};
        return {
            accessToken: user.access_token,
            tokenType: user.token_type,
            scope: user.scope,
            expired: user.expired,
            profile: {
                userId: user.profile.sub,
                name: user.profile.name,
                email: user.profile.email,
                website: user.profile.website
            }
        };
    }).catch(function (e) {
        console.error("getUserInfo error", e.message);
    });
};

function clearState() {
    mgr.clearStaleState().then(function () {
        console.log("clearStateState success");
    }).catch(function (e) {
        console.error("clearStateState error", e.message);
    });
}

function getUser() {
    mgr.getUser().then(function (user) {
        console.log("got user", user);
    }).catch(function (err) {
        console.error(err);
    });
}

function removeUser() {
    mgr.removeUser().then(function () {
        console.log("user removed");
    }).catch(function (err) {
        console.error(err);
    });
}

function startSigninMainWindow() {
    mgr.signinRedirect({ state: 'some data' }).then(function () {
        console.log("signinRedirect done");
    }).catch(function (err) {
        console.error(err);
    });
}

function endSigninMainWindow() {
    return mgr.signinRedirectCallback().then(function (user) {
        console.log("signed in", user);
        return user;
    }).catch(function (err) {
        console.error(err);
    });
}

function startSigninMainWindowDiffCallbackPage() {
    mgr.signinRedirect({ state: 'some data', redirect_uri: url + '/code-identityserver-sample-callback.html' }).then(function () {
        console.log("signinRedirect done");
    }).catch(function (err) {
        console.error(err);
    });
}

function popupSignin() {
    mgr.signinPopup({ state: 'some data' }).then(function (user) {
        console.log("signed in", user);
    }).catch(function (err) {
        console.error(err);
    });
}

function popupSignout() {
    mgr.signoutPopup({ state: 'some data' }).then(function () {
        console.log("signed out");
    }).catch(function (err) {
        console.error(err);
    });
}

function iframeSignin() {
    mgr.signinSilent({ state: 'some data' }).then(function (user) {
        console.log("signed in", user);
    }).catch(function (err) {
        console.error(err);
    });
}

function querySessionStatus() {
    mgr.querySessionStatus().then(function (status) {
        console.log("user's session status", status);
    }).catch(function (err) {
        console.error(err);
    });
}

function startSignoutMainWindow() {
    //mgr.signoutRedirect({ state: 'some data' }).then(function (resp) {
    mgr.signoutRedirect().then(function(resp) {
        console.log("signed out", resp);
    }).catch(function (err) {
        console.error(err);
    });
};

function endSignoutMainWindow() {
    mgr.signoutRedirectCallback().then(function (resp) {
        console.log("signed out", resp);
    }).catch(function (err) {
        console.error(err);
    });
};

function log(message) {
    console.log(message);
    return true;
}