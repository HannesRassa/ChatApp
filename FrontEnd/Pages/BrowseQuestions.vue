<template> 
  <div class="question-list">
    <h2>Available Questions</h2>
    <ul v-if="questions.length">
      <li v-for="question in questions" :key="question.id">
        {{ question.questionText }}
      </li>
    </ul>
    <p v-else>No questions available.</p>
    <div v-if="errorMessage" class="error">{{ errorMessage }}</div>
    <div class="top-left">
      <li><NuxtLink to="/">Home</NuxtLink></li>
    </div>
  </div>
</template>

<script lang="ts">
import axios from 'axios';
import { defineComponent } from 'vue';

interface Question {
  id: number;
  questionText: string;
}

export default defineComponent({
  data() {
    return {
      questions: [] as Question[],
      errorMessage: '',
    };
  },
  created() {
    this.fetchQuestions();
  },
  methods: {
    async fetchQuestions() {
      try {
        const response = await axios.get('https://localhost:7269/Backend/Question'); 
        this.questions = response.data;
        console.log('Fetched questions:', this.questions); 
      } catch (error) {
        if (axios.isAxiosError(error)) {
          this.errorMessage = error.response?.data?.message || 'Failed to fetch questions.';
        } else {
          this.errorMessage = 'An unexpected error occurred.';
        }
        console.error("Error fetching questions:", this.errorMessage);
      }
    }
  }
});
</script>

<style scoped>
/* Container Styling */
.question-list {
  padding: 20px;
  max-width: 600px;
  margin: 40px auto;
  background-color: #f4f0ff; /* Light lavender background */
  border: 1px solid #d3bce7; /* Light purple border */
  border-radius: 10px;
  box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);
  text-align: center;
}

/* Title Styling */
.question-list h2 {
  font-size: 1.8rem;
  color: #5b2c6f; /* Dark purple */
  margin-bottom: 20px;
}

/* Question List Styling */
.question-list ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.question-list li {
  font-size: 1.2rem;
  color: #7d3c98; /* Dark purple for text */
  background-color: #e0d4f7; /* Light purple background */
  margin-bottom: 10px;
  padding: 10px 15px;
  border-radius: 8px;
  transition: background-color 0.3s, transform 0.2s;
}

.question-list li:hover {
  background-color: #d3bce7; /* Slightly darker purple on hover */
  transform: translateY(-3px); /* Subtle lift effect */
  cursor: pointer;
}

/* No Questions Message */
.question-list p {
  font-size: 1rem;
  color: #a569bd; /* Medium purple */
  margin-top: 20px;
}

/* Error Message */
.error {
  color: #e74c3c; /* Bright red for errors */
  margin-top: 15px;
  font-size: 1rem;
}

/* Top Left Navigation */
.top-left {
  position: absolute;
  top: 10px;
  left: 10px;
}

.top-left li {
  list-style: none;
  font-size: 1rem;
}

.top-left a {
  text-decoration: none;
  color: #5b2c6f; /* Dark purple */
  font-weight: bold;
  border: 1px solid #d3bce7;
  padding: 5px 10px;
  border-radius: 5px;
  transition: background-color 0.3s, color 0.3s;
}

.top-left a:hover {
  background-color: #7d3c98; /* Darker purple */
  color: white;
}
</style>
