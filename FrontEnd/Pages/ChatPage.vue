<template>
  <div>
    <button v-if="!countdownActive" @click="startCountdown">Start Countdown</button>

    <!-- Countdown display -->
    <div v-if="countdownText">{{ countdownText }}</div>

    <!-- Question display after countdown ends -->
    <div v-if="question">
      <h3>Question: {{ question.questionText }}</h3>
      <ul>
        <li v-for="answer in question.possibleAnswers" :key="answer">
          <label>
            <input
              type="radio"
              name="answer"
              :value="answer"
              v-model="selectedAnswer"
            />
            {{ answer }}
          </label>
        </li>
      </ul>
      <button @click="submitAnswer">Submit Answer</button>
    </div>

    <!-- Error display -->
    <div v-if="error" class="error">{{ error }}</div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      countdownSteps: ["Ready", "Steady", "Go!"],
      countdownIndex: -1,
      countdownText: "",
      countdownActive: false,
      currentQuestionId: 1, // Starting with question ID 1
      question: null,
      selectedAnswer: "",
      error: null,
    };
  },
  methods: {
    startCountdown() {
      this.countdownActive = true;
      this.countdownIndex = 0;
      this.updateCountdown();
    },
    updateCountdown() {
      if (this.countdownIndex < this.countdownSteps.length) {
        this.countdownText = this.countdownSteps[this.countdownIndex];
        setTimeout(() => {
          this.countdownIndex++;
          this.updateCountdown();
        }, 1000); // 1-second delay between each step
      } else {
        this.countdownText = "";
        this.countdownActive = false;
        this.fetchQuestion(); // Fetch the first question after the countdown
      }
    },
    fetchQuestion() {
      axios
        .get(`http://localhost:5180/Backend/Question/${this.currentQuestionId}`)
        .then((response) => {
          this.question = response.data;
          this.error = null;
          this.selectedAnswer = ""; // Reset the selected answer
        })
        .catch((error) => {
          this.error = "Error fetching the question.";
          this.question = null;
          if (error.response && error.response.status === 404) {
            this.error = "Question not found!";
          }
        });
    },
    submitAnswer() {
      if (!this.selectedAnswer) {
        this.error = "Please select an answer!";
        return;
      }

      // Call backend to submit the answer
      axios
        .post("http://localhost:5180/Backend/Game/SubmitAnswer", {
          questionId: this.currentQuestionId,
          answer: this.selectedAnswer,
        })
        .then((response) => {
          console.log("Answer submitted successfully!", response.data);

          // Move to the next question
          this.currentQuestionId++;
          this.fetchQuestion(); // Fetch the next question
        })
        .catch((error) => {
          console.error("Error submitting answer:", error);
          this.error = "Error submitting the answer.";
        });
    },
  },
};
</script>

<style scoped>
button {
  margin-bottom: 20px;
}

div {
  font-size: 2rem;
  font-weight: bold;
}

.error {
  color: red;
  font-weight: bold;
}
</style>
