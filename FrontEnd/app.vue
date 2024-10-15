<template>
  <div class="quiz-container">
    <!-- Title -->
    <h1 class="quiz-title">Quiz Game</h1>

    <!-- Current question display -->
    <div v-if="!completed" class="question-container">
      <h2>{{ currentQuestion }}</h2>

      <!-- Logic for the first two questions -->
      <div v-if="currentQuestionIndex < 2">
        <form @submit.prevent="submitAnswer" class="answer-form">
          <input
            type="text"
            v-model="userAnswer"
            placeholder="Type your answer here"
            class="answer-input"
          />
          <button type="submit" class="submit-btn">Submit</button>
        </form>
      </div>

      <!-- Logic for the third question (funny answer round) -->
      <div v-else-if="currentQuestionIndex === 2">
        <form @submit.prevent="submitFunnyAnswer" class="answer-form">
          <input
            type="text"
            v-model="userAnswer"
            placeholder="Type your funny answer here"
            class="answer-input"
          />
          <button type="submit" class="submit-btn">Submit Funny Answer</button>
        </form>
      </div>

      <!-- Logic to choose which answer is funnier -->
      <div v-if="funnyAnswersSubmitted" class="choose-funnier-section">
        <p>Who gave the funnier answer?</p>
        <p>Your answer: {{ userFunnyAnswer }}</p>
        <p>Other player's answer: {{ otherPlayerFunnyAnswer }}</p>
        <button @click="chooseFunnier('player')" class="choose-btn">My answer</button>
        <button @click="chooseFunnier('otherPlayer')" class="choose-btn">Other player's answer</button>
      </div>
    </div>

    <!-- Messages showing result of answers for the first two questions -->
    <div v-if="submitted && !completed && currentQuestionIndex < 2" class="result-message">
      <p v-if="isCorrect" class="correct-answer">Correct! 🎉 You've earned 100 points!</p>
      <p v-else class="incorrect-answer">Incorrect, try again. ❌</p>
    </div>

    <!-- Quiz completion section -->
    <div v-if="completed" class="completion-section">
      <h2>Quiz Completed! 🎉</h2>
      <p class="score">Your total score: <strong>{{ score }}</strong></p>

      <!-- Display other player's score and answers -->
      <h3 class="other-player-score">Other player's total score: <strong>{{ otherPlayerScore }}</strong></h3>
      <ul class="other-player-answers">
        <li v-for="(answer, index) in otherPlayerAnswers" :key="index">
          Question {{ index + 1 }}: <strong>{{ answer }}</strong>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      questions: [
        {
          text: "What is the capital of France?",
          correctAnswer: "Paris",
        },
        {
          text: "What is 2 + 2?",
          correctAnswer: "4",
        },
        {
          text: "Give a funny answer to: 'Why did the chicken cross the road?'",  // Third question
          correctAnswer: null,  // No predefined correct answer, it's subjective
        },
      ],
      currentQuestionIndex: 0,
      userAnswer: "",
      submitted: false,
      isCorrect: false,
      completed: false,
      funnyAnswersSubmitted: false,
      score: 0,  // Player's score
      otherPlayerScore: 0,  // Other player's score
      otherPlayerAnswers: [],  // Other player's answers
      otherPlayerFunnyAnswer: "To get to the other side!",  // Predefined funny answer for other player
      userFunnyAnswer: "",  // Player's funny answer
    };
  },
  computed: {
    // Get the current question
    currentQuestion() {
      return this.questions[this.currentQuestionIndex].text;
    },
  },
  methods: {
    // Function to submit answers for the first two questions
    submitAnswer() {
      const currentQuestion = this.questions[this.currentQuestionIndex];
      this.submitted = true;

      // Check if the answer is correct (for the first two questions)
      this.isCorrect = this.userAnswer.toLowerCase() === currentQuestion.correctAnswer?.toLowerCase();

      // Simulate other player's correct answer
      const otherPlayerAnswer = currentQuestion.correctAnswer;
      this.otherPlayerAnswers.push(otherPlayerAnswer);
      
      // Award points for correct answers
      if (this.isCorrect) {
        this.score += 100;  // Player earns points for correct answer
      } else {
        this.otherPlayerScore += 100;  // Other player earns points for correct answer
      }
      
      // Move to the next question regardless of the answer correctness
      this.userAnswer = "";  // Clear input field
      this.submitted = false;  // Reset submission state
      this.currentQuestionIndex++;  // Move to the next question

      // Check if quiz is completed
      if (this.currentQuestionIndex >= this.questions.length) {
        this.completed = true;  // Mark quiz as completed
      }
    },
    // Function to submit funny answers for the third question
    submitFunnyAnswer() {
      this.userFunnyAnswer = this.userAnswer;  // Save player's funny answer
      this.otherPlayerAnswers.push(this.otherPlayerFunnyAnswer);  // Add other player's funny answer
      this.userAnswer = "";  // Clear input field
      this.funnyAnswersSubmitted = true;  // Mark funny answers as submitted
    },
    // Function to choose which funny answer is better
    chooseFunnier(choice) {
      if (choice === 'player') {
        this.score += 100;  // Player gets extra points for their answer
      } else if (choice === 'otherPlayer') {
        this.otherPlayerScore += 100;  // Other player gets extra points
      }

      this.completed = true;  // End the game
    }
  },
};
</script>

<style scoped>
/* Container styling */
.quiz-container {
  max-width: 500px;
  margin: 0 auto;
  padding: 20px;
  background-color: #f9f9f9;
  box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
  border-radius: 10px;
  font-family: 'Arial', sans-serif;
  text-align: center;
}

.quiz-title {
  color: #333;
  font-size: 24px;
  margin-bottom: 20px;
}

.question-container h2 {
  color: #444;
  font-size: 20px;
  margin-bottom: 15px;
}

/* Input form styling */
.answer-form {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 20px;
}

.answer-input {
  padding: 10px;
  font-size: 16px;
  border-radius: 5px;
  border: 1px solid #ddd;
  width: 100%;
  box-sizing: border-box;
}

.submit-btn, .choose-btn {
  padding: 10px;
  background-color: #4CAF50;
  color: white;
  font-size: 16px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.submit-btn:hover, .choose-btn:hover {
  background-color: #45a049;
}

/* Result message styling */
.result-message {
  margin-bottom: 20px;
}

.correct-answer {
  color: #4CAF50;
  font-size: 18px;
  font-weight: bold;
}

.incorrect-answer {
  color: #e74c3c;
  font-size: 18px;
  font-weight: bold;
}

/* Completion section styling */
.completion-section {
  margin-top: 20px;
}

.score, .other-player-score {
  font-size: 18px;
  margin-bottom: 10px;
}

.other-player-answers {
  list-style: none;
  padding: 0;
}

.other-player-answers li {
  background-color: #eee;
  padding: 10px;
  margin: 5px 0;
  border-radius: 5px;
  font-size: 16px;
}
</style>
