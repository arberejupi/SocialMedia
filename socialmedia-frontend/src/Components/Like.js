import React, { useEffect, useState } from 'react';
import "./Like.css";
const Like = ({ postId, userId, token }) => {
  const [likeCount, setLikeCount] = useState(0);
  const [isLiked, setIsLiked] = useState(false);

  useEffect(() => {
    fetchLikeCount();
    checkIfLiked();
    
  }, [postId, userId, token]);

  const fetchLikeCount = async () => {
    try {
      const response = await fetch(
        `https://localhost:7069/api/likes/post/${postId}?userId=${userId}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
  
      if (response.ok) {
        const data = await response.json();
        setLikeCount(data);
      } else {
        throw new Error('Failed to fetch like count');
      }
    } catch (error) {
      console.log('Error occurred while fetching like count:', error);
    }
  };
  

  const checkIfLiked = async () => {
    try {
      const response = await fetch(`https://localhost:7069/api/likes/post/${postId}/check`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
  
      if (response.ok) {
        const data = await response.json();
        setIsLiked(data);
      } else {
        throw new Error('Failed to check if liked');
      }
    } catch (error) {
      console.log('Error occurred while checking if liked:', error);
    }
  };
  
  const likePost = async () => {
    try {
      const response = await fetch('https://localhost:7069/api/likes', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          postId: postId,
          userId: userId,
        }),
      });

      if (response.ok) {
        setIsLiked(true);
        setLikeCount(likeCount + 1);
      } else {
        throw new Error('Failed to like post');
      }
    } catch (error) {
      console.log('Error occurred while liking post:', error);
    }
  };

  const unlikePost = async () => {
    try {
      const response = await fetch(`https://localhost:7069/api/likes/post/${postId}/unlike?userId=${userId}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.ok) {
        setIsLiked(false);
        setLikeCount(likeCount - 1);
      } else {
        throw new Error('Failed to unlike post');
      }
    } catch (error) {
      console.log('Error occurred while unliking post:', error);
    }
  };

  return (
    <div className='likeBtn'>
      <span>{likeCount} Likes</span>
      {isLiked ? (
        <button onClick={unlikePost}>Unlike</button>
      ) : (
        <button onClick={likePost}>Like</button>
      )}
    </div>
  );
};

export default Like;
