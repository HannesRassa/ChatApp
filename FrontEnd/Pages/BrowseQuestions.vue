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
        const response = await axios.get('http://localhost:5180/Backend/Question'); 
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
.question-list {
  padding: 20px;
  max-width: 600px;
  margin: auto;
}

.error {
  color: red;
  margin-top: 10px;
}
</style>
