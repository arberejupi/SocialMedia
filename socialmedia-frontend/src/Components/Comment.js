import React, { useEffect, useState } from 'react';

const Comment = ({ postId, userId, token }) => {
  const [content, setContent] = useState('');
  const [isCommenting, setIsCommenting] = useState(false);
  const [comments, setComments] = useState([]);
  const [editingCommentId, setEditingCommentId] = useState(null);
  const [commentCount, setCommentCount] = useState(0); // New state for comment count
  

  useEffect(() => {
    fetchComments();
  }, [postId]);

  const fetchComments = async () => {
    try {
      const response = await fetch(`https://localhost:7069/api/comments/post/${postId}`);
      if (response.ok) {
        const commentsData = await response.json();
        setComments(commentsData);
        setCommentCount(commentsData.length); // Update comment count
      } else if (response.status === 404) {
        setComments([]); // Set an empty array to indicate no comments
        setCommentCount(0); // Reset comment count
      } else {
        throw new Error('Failed to fetch comments');
      }
    } catch (error) {
      console.log('Error occurred while fetching comments:', error);
    }
  };
  const createComment = async () => {
    try {
      const response = await fetch('https://localhost:7069/api/comments', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          postId: postId,
          content: content,
        }),
      });

      if (response.ok) {
        console.log('Comment created successfully');
        setContent('');
        setIsCommenting(false);
        fetchComments();
      } else {
        throw new Error('Failed to create comment');
      }
    } catch (error) {
      console.log('Error occurred while creating comment:', error);
    }
  };

  const updateComment = async (commentId, updatedContent) => {
    try {
      const response = await fetch(`https://localhost:7069/api/comments/${commentId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          postId:postId,
          content: updatedContent,
        }),
      });

      if (response.ok) {
        console.log('Comment updated successfully');
        setEditingCommentId(null); // Reset the editing state
        fetchComments();
      } else {
        throw new Error('Failed to update comment');
      }
    } catch (error) {
      console.log('Error occurred while updating comment:', error);
    }
  };

  const deleteComment = async (commentId) => {
    try {
      const response = await fetch(`https://localhost:7069/api/comments/${commentId}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.ok) {
        console.log('Comment deleted successfully');
        fetchComments();
      } else {
        throw new Error('Failed to delete comment');
      }
    } catch (error) {
      console.log('Error occurred while deleting comment:', error);
    }
  };

  const handleContentChange = (event) => {
    setContent(event.target.value);
  };

  const handleCommentClick = () => {
    setIsCommenting(true);
  };

  const handleEditClick = (commentId, commentContent) => {
    setEditingCommentId(commentId);
    setContent(commentContent);
  };

  return (
    <div>
       <div>
       <span>{commentCount} Comments</span>
        {!isCommenting ? (
          <button onClick={handleCommentClick}>Comment</button>
        ) : (
          <div>
            <input type="text" value={content} onChange={handleContentChange} />
            <button onClick={createComment}>Send</button>
          </div>
        )}
      </div>
      <div>

        {comments.map((comment) => (
          <div key={comment.id}>
            {editingCommentId === comment.id ? (
              <div>
                <input type="text" value={content} onChange={handleContentChange} />
                <button onClick={() => updateComment(comment.id, content)}>Save</button>
              </div>
            ) : (
              <div>
                <p>{comment.content}</p>
                {comment.userId === userId && (
                  <div>
                    <button onClick={() => handleEditClick(comment.id, comment.content)}>
                      Edit
                    </button>
                    <button onClick={() => deleteComment(comment.id)}>Delete</button>
                  </div>
                )}
              </div>
            )}
          </div>
        ))}
      </div>
     
    </div>
  );
};

export default Comment;
