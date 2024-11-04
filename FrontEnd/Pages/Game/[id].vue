<template>
  <div class="game-container">
    <div class="game-id">
      <p>Game ID: {{ gameData.id }}</p>
    </div>

    <!-- Question Text -->
    <div class="question-box">
      <p>{{ selectedQuestion.questionText }}</p>
    </div>

    <!-- Answer Box -->
    <div class="answer-box">
      <input
        v-model="answer"
        placeholder="Type your answer here"
        class="answer-input"
      />
    </div>

    <!-- Submit Button -->
    <button @click="submitAnswer" class="submit-button">Submit Answer</button>

    <!-- Player Points -->
    <div class="points">
      <p>Points: {{ gameData.id }}</p>
      <!--dont change right now!!!-->
    </div>

    <!-- Opponent Section -->
    <div class="opponent">
      <p>Opponent</p>
      <div class="opponent-icon">✓</div>
    </div>

    <!-- Summary Button -->
    <button @click="showSummary" class="summary-button">Show Summary</button>

    <!-- Summary Modal -->
    <div
      v-if="isSummaryVisible"
      class="modal-overlay"
      @click.self="isSummaryVisible = false"
    >
      <div class="modal-content">
        <h3>Answer Summary</h3>
        <ul>
          <li v-for="(item, index) in answeredQuestions" :key="index">
            <strong>Question:</strong> {{ item.question }} <br />
            <strong>Your Answer:</strong> {{ item.yourAnswer }} <br />
            <strong>Fake Player Answer:</strong> {{ item.fakeAnswer }} <br />
          </li>
        </ul>
        <button @click="isSummaryVisible = false" class="close-button">
          Close
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRoute } from "vue-router";
import { ref, onMounted } from "vue";
import axios from "axios";

const route = useRoute();
const gameData = ref({
  id: "",
  player: {
    points: 0,
  },
});
const answer = ref("");
const selectedQuestion = ref({});
const questionIds = ref([]);
const answeredQuestions = ref([]); // Track answered questions
const rounds = 5;
const currentRound = ref(0);
const isSummaryVisible = ref(false); // Control summary modal visibility

const fetchGameData = async () => {
  const { id } = route.params;
  console.log("Fetching data for Game ID:", id);
  try {
    const response = await axios.get(
      `http://localhost:5180/Backend/Game/${id}`
    );
    console.log("Response data:", response.data);
    gameData.value = response.data;
    await fetchAllQuestionIds();
  } catch (error) {
    console.error("Error fetching game data:", error);
  }
};

const fetchAllQuestionIds = async () => {
  try {
    const response = await axios.get(`http://localhost:5180/Backend/question/`);
    console.log("Fetched question IDs:", response.data);
    questionIds.value = response.data.map((question) => question.id);
    await fetchRandomQuestion();
  } catch (error) {
    console.error("Error fetching question IDs:", error);
  }
};

const fetchRandomQuestion = async () => {
  if (questionIds.value.length === 0) {
    console.error("No question IDs available to fetch a question.");
    return;
  }

  const randomId =
    questionIds.value[Math.floor(Math.random() * questionIds.value.length)];
  console.log("Fetching question with ID:", randomId);
  try {
    const response = await axios.get(
      `http://localhost:5180/Backend/question/${randomId}`
    );
    console.log("Fetched question:", response.data);
    selectedQuestion.value = response.data;
  } catch (error) {
    console.error("Error fetching question:", error);
  }
};

const submitAnswer = async () => {
  if (answer.value.trim() === "") {
    alert("Please enter an answer before submitting.");
    return;
  }

  try {
    console.log("Submitting Answer:", answer.value);

    // Store the answered question and fake answer
    answeredQuestions.value.push({
      question: selectedQuestion.value.questionText,
      yourAnswer: answer.value,
      fakeAnswer: generateFakeAnswer(), // Generate a fake answer
    });

    answer.value = "";
    currentRound.value += 1;

    if (currentRound.value < rounds) {
      await fetchRandomQuestion();
    } else {
      alert("Game Over! You have completed all rounds.");
    }
  } catch (error) {
    console.error("Error submitting answer:", error);
    alert("An error occurred while submitting your answer. Please try again.");
  }
};

// Generate a fake answer from a fake player
const generateFakeAnswer = () => {
  const fakeAnswers = [
    "Fake Answer1",
    "Fake Answer2",
    "Fake Answer3",
    "Fake Answer4",
    "Fake Answer5",
    "Fake Answer6",
  ];
  return fakeAnswers[Math.floor(Math.random() * fakeAnswers.length)];
};

// Show the answer summary modal
const showSummary = () => {
  isSummaryVisible.value = true;
};

onMounted(() => {
  fetchGameData();
});
</script>

<style scoped>
/* General Game Container Styles */
.game-container {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
  background-color: #ffffff;
  border-radius: 10px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  font-family: "Arial", sans-serif;
}

/* Game ID and Points Styles */
.game-id,
.points {
  margin-bottom: 15px;
  padding: 12px;
  border-radius: 6px;
  background-color: #f1f1f1;
  border: 1px solid #e1e1e1;
  font-size: 1.1em;
  color: #333;
}

/* Question Box Styles */
.question-box {
  margin-bottom: 20px;
  padding: 15px;
  border-radius: 8px;
  background-color: #f8f9fa;
  border: 1px solid #dee2e6;
  font-size: 1.2em;
  font-weight: bold;
  color: #212529;
}

/* Answer Box Styles */
.answer-box {
  margin-bottom: 20px;
}

/* Answer Input Styles */
.answer-input {
  width: 100%;
  padding: 12px;
  border: 1px solid #ced4da;
  border-radius: 5px;
  font-size: 1em;
  transition: border-color 0.3s;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.answer-input:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 5px rgba(0, 123, 255, 0.5);
}

/* Submit Button Styles */
.submit-button {
  padding: 12px 25px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1em;
  transition: background-color 0.3s, transform 0.2s;
  box-shadow: 0 2px 5px rgba(0, 123, 255, 0.2);
}

.submit-button:hover {
  background-color: #0056b3;
  transform: translateY(-2px);
}

.submit-button:active {
  transform: translateY(1px);
}

/* Opponent Section Styles */
.opponent {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-top: 20px;
}

.opponent-icon {
  margin-top: 5px;
  font-size: 1.5em;
  color: #28a745;
}

/* Summary Button Styles */
.summary-button {
  margin-top: 20px;
  padding: 12px 20px;
  background-color: #17a2b8;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1em;
  transition: background-color 0.3s, transform 0.2s;
}

.summary-button:hover {
  background-color: #138496;
  transform: translateY(-2px);
}

.summary-button:active {
  transform: translateY(1px);
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: #ffffff;
  padding: 25px;
  border-radius: 10px;
  width: 90%;
  max-width: 500px;
  box-shadow: 0 4px 30px rgba(0, 0, 0, 0.2);
  animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.close-button {
  margin-top: 15px;
  padding: 10px 20px;
  background-color: #dc3545;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1em;
  transition: background-color 0.3s;
}

.close-button:hover {
  background-color: #c82333;
}
</style>
