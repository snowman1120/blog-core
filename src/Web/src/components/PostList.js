import React, { Component } from "react";
import moment from "moment";
import { Link } from "react-router-dom";

export default class PostList extends Component {
  render() {
    const { ids, posts } = this.props;
    return (
      <div>
        {ids.map((id, index) =>
          <div key={id} className="blog-post">
            <h2 className="blog-post-title">
              <Link to={`/post/${id}`}>
                {posts[id].title}
              </Link>
            </h2>
            <p className="blog-post-meta">
              {moment(posts[id].created_at).format(
                "MMMM Do, YYYY"
              )}&nbsp;by&nbsp;
              <a href="#">
                {`${posts[id].author.givenName} ${posts[id].author.familyName}`}
              </a>
            </p>

            <p>{posts[id].excerpt}</p>
          </div>
        )}
      </div>
    );
  }
}
