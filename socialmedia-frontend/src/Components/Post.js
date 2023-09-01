import React, { useEffect, useState } from 'react';
import Like from './Like';
import Comment from './Comment';
import './Post.css';

const Post = ({ userId, username, token }) => {
  const [posts, setPosts] = useState([]);
  const [newPostContent, setNewPostContent] = useState('');
  const [usernames, setUsernames] = useState({}); // New state to store usernames

  useEffect(() => {
    fetchPosts();
    fetchUsernames(); // Fetch usernames when the component mounts
  }, [userId]);

  const fetchUsernames = async () => {
    try {
      const response = await fetch(`https://localhost:7069/api/Auth/usernames`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.ok) {
        const data = await response.json();
        setUsernames(data); // Store usernames in state
      } else {
        throw new Error('Failed to fetch usernames');
      }
    } catch (error) {
      console.log('Error occurred while fetching usernames:', error);
    }
  };
  
  const fetchPosts = async () => {
    try {
      const response = await fetch(`https://localhost:7069/api/posts?userId=${userId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.ok) {
        const data = await response.json();
        setPosts(data);
      } else {
        throw new Error('Failed to fetch posts');
      }
    } catch (error) {
      console.log('Error occurred while fetching posts:', error);
    }
  };

  const createPost = async () => {
    try {
      const response = await fetch('https://localhost:7069/api/posts/create', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          userId: userId,
          content: newPostContent,
          createdAt: new Date().toISOString(),
        }),
      });

      if (response.ok) {
        const data = await response.json();
        setPosts([...posts, data]);
        setNewPostContent('');
      } else {
        throw new Error('Failed to create post');
      }
    } catch (error) {
      console.log('Error occurred while creating post:', error);
    }
  };

  const handleComment = async (postId) => {
    try {
      const response = await fetch(`https://localhost:7069/api/posts/${postId}/comments`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          userId: userId,
          content: 'Your comment here',
          createdAt: new Date().toISOString(),
        }),
      });

      if (response.ok) {
        const data = await response.json();
        // Update the posts array to include the new comment
        const updatedPosts = posts.map((post) => {
          if (post.id === postId) {
            return {
              ...post,
              comments: post.comments ? [...post.comments, data] : [data],
            };
          }
          return post;
        });
        setPosts(updatedPosts);
      } else {
        throw new Error('Failed to create comment');
      }
    } catch (error) {
      console.log('Error occurred while creating comment:', error);
    }
  };

  return (
    <div>
      <div className="post-form">
        <h2>Create a Post</h2>
        <input
          type="text"
          value={newPostContent}
          onChange={(e) => setNewPostContent(e.target.value)}
          placeholder="Write your post here..."
        />
        <button onClick={createPost}>Create Post</button>
      </div>
      <div className="post-list">
        {posts.map((post) => (
          <div key={post.id} className="post">
            <p>{post.content}</p>
            <div className="post-info">
              <p>Posted by {usernames[post.userId]}</p> {/* Display username instead of userId */}
              <p>Posted on {new Date(post.createdAt).toLocaleString()}</p>
            </div>
            <div className='likeComment'>
            <Like postId={post.id} userId={userId} token={token} />
            <div className="comment-section">
            <Comment postId={post.id} userId={userId} token={token} />
            </div>
            </div>
           {/* <div className="comment-list">
              {post.comments && Array.isArray(post.comments) ? (
                post.comments.map((comment) => (
                  <Comment
                    key={comment.id}
                    postId={post.id}
                    commentId={comment.id}
                    userId={userId}
                    token={token}
                  />
                ))
              ) : null}
                </div>*/}
          </div>
        ))}
      </div>
    </div>
  );
};

export default Post;
